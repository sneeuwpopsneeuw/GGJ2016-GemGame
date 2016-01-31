using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
using System.Linq;
using Random = System.Random;

public class Gem : MonoBehaviour
{
    public float angle = 45;
    public float speed = 2;
    public float spawnTime;
    public List<SpawnGem> gems = new List<SpawnGem>();
    public GameObject cockBlockerPrefab;
    public Transform smallGemParent;
    public PlayerId playerId;
    public string leftKey = "lk", rightKey = "rk", upKey = "uk", downKey = "dk";
    public float targetAngle;
    public float minGemWidth = 231, maxGemWidth = 231;
    public int health = 100;
    public Sprite[] healthSprites;
    public Transform[] arrowPosition;
    private int arrowIndex = 2;
    public GameObject deathScreen;
    public List<SpawnGem> spawnedGems = new List<SpawnGem>();
    public GameObject arrowPrefab;
    private GameObject arrowInstance;
    private float spawnTimer;
    private float minDistance = 2.3f;
    private int maxRows = 5;
    private float blockHeight = .8f;
    public bool paused = false;
    public bool rotating;
    public PixelWarManager pwm;
    public float speedMultiplier = .02f;
    public float spawnTimeMultiplier = 1f;
    public AudioSource swoosh;
    public AudioSource pling;
    public AudioClip plingClip;
    public AudioClip swooshClip;
    RaycastHit2D[] hit = new RaycastHit2D[2];
    Random rand;

    void Start()
    {
        DOTween.Init();
        rand = new Random(System.DateTime.Now.Millisecond * UnityEngine.Random.Range(0,100));
        smallGemParent.transform.position = transform.position;
        spawnTimer = spawnTime;
        arrowInstance = Instantiate(arrowPrefab);
        arrowInstance.transform.position = arrowPosition[arrowIndex].position;
    }

    void Update()
    {
        if (!paused)
        {
            speed += Time.deltaTime * speedMultiplier;
            spawnTime = Mathf.Lerp(2, 1, Time.deltaTime * spawnTimeMultiplier);
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnTime)
            {
                spawnTimer = 0;
                //SpawnCockblocker();
                SpawnSmallGem();
            }

            if (Input.GetButtonDown(leftKey + "_" + playerId.ToString()))
            {
                RotateGem(angle);
            }

            if (Input.GetButtonDown(rightKey + "_" + playerId.ToString()))
            {
                RotateGem(-angle);
            }

            if (Input.GetButtonDown(upKey + "_" + playerId.ToString()))
            {
                arrowIndex = Mathf.Clamp(arrowIndex - 1, 0, arrowPosition.Length - 1);
                MoveArrow(arrowIndex);
            }

            if (Input.GetButtonDown(downKey + "_" + playerId.ToString()))
            {
                arrowIndex = Mathf.Clamp(arrowIndex + 1, 0, arrowPosition.Length - 1);
                MoveArrow(arrowIndex);
            }

            for (int i = 0; i < spawnedGems.Count; i++)
            {
                SpawnGem sg = spawnedGems[i];
                CheckCombo(sg);
                sg.col.enabled = false;
                int count = Physics2D.RaycastNonAlloc(sg.prefab.transform.position, -sg.prefab.transform.up, hit, .4f);
                if (count > 1)
                {
                    sg.moving = false;
                    bool succes = false;
                    RaycastHit2D[] hits = new RaycastHit2D[20];
                    count = Physics2D.RaycastNonAlloc(sg.prefab.transform.position, -sg.prefab.transform.up, hits, (blockHeight * (maxRows)));
                    List<GameObject> hitObj = new List<GameObject>();
                    for (int x = 0; x < count; x++)
                    {
                        if(!hitObj.Contains(hits[x].transform.gameObject))
                        {
                            hitObj.Add(hits[x].transform.gameObject);
                        }
                        if (hits[x].collider.gameObject == gameObject)
                        {
                            succes = true;
                            break;
                        }
                    }
                    if (succes)
                    {
                        continue;
                    }
                    else
                    {
                        if (hitObj.Count> 5)
                        {
                            spawnedGems.Remove(sg);
                            Destroy(sg.prefab.transform.parent.gameObject);
                            TakeDamage(25);
                            continue;
                        }
                    }
                }
                else
                {
                    sg.prefab.transform.position += -sg.prefab.transform.up * Time.deltaTime * speed;
                    sg.moving = true;
                }
                sg.col.enabled = true;
                Vector2 v = sg.sprite.dimensions;
                v.x = Mathf.Lerp(minGemWidth, maxGemWidth, (Vector3.Distance(transform.position, sg.prefab.transform.position) - minDistance) / ((minDistance + blockHeight * (maxRows - 1)) - minDistance));
                sg.sprite.dimensions = v;
                Vector2 v2 = sg.col.size;
                v2.x = v.x / 100f;
                sg.col.size = v2;
            }
            hit = new RaycastHit2D[2];
        }
    }

    public void MoveArrow(int i)
    {
        arrowInstance.transform.DOMove(arrowPosition[i].position, .2f);
    }

    public void RotateGem(float angle)
    {
        if (!rotating)
        {
            swoosh.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            swoosh.PlayOneShot(swooshClip);
            //transform.Rotate(0, 0, angle);
            rotating = true;
            for (int i = 0; i < spawnedGems.Count; i++)
            {
                SpawnGem sg = spawnedGems[i];
                sg.col.enabled = false;
                int count = Physics2D.RaycastNonAlloc(sg.prefab.transform.position, -sg.prefab.transform.up, hit, .45f);
                count += Physics2D.RaycastNonAlloc(sg.prefab.transform.position, sg.prefab.transform.right, hit, 2.6f);
                count += Physics2D.RaycastNonAlloc(sg.prefab.transform.position, -sg.prefab.transform.right, hit, 2.6f);
                if (count > 3)
                {
                    sg.targetAngle += angle;
                    sg.prefab.transform.parent.DOLocalRotate(new Vector3(0, 0, angle), .2f, RotateMode.LocalAxisAdd);
                }
                sg.col.enabled = true;
            }
            targetAngle += angle;
            transform.DOLocalRotate(new Vector3(0, 0, angle), .2f, RotateMode.LocalAxisAdd).OnComplete(() => rotating = false);
        }
    }

    public void CheckCombo(SpawnGem gem)
    {
        if (gem.type != GemType.Cockblocker)
        {
            gem.col.enabled = false;
            RaycastHit2D[] hits = new RaycastHit2D[5];
            List<SpawnGem> gems = new List<SpawnGem>();
            gems.Add(gem);
            Vector2[] directions = new Vector2[] { gem.prefab.transform.up, -gem.prefab.transform.up, gem.prefab.transform.right, -gem.prefab.transform.right };
            float[] distances = new float[] { .45f, .45f, 3f, 3f };
            for (int i = 0; i < directions.Length; i++)
            {
                int count = Physics2D.RaycastNonAlloc(gem.prefab.transform.position, directions[i], hits, distances[i]);
                for (int x = 0; x < count; x++)
                {
                    if (hits[x].collider.gameObject != gem.prefab)
                    {
                        if (hits[x].collider.gameObject.tag == gem.prefab.gameObject.tag)
                        {

                            SpawnGem foundGem = spawnedGems.First(s => s.prefab == hits[x].collider.gameObject);
                            if (!foundGem.moving && !gem.moving)
                            {
                                if (!gems.Contains(foundGem))
                                {
                                    gems.Add(foundGem);
                                }
                            }
                        }
                    }
                }
            }
            gem.col.enabled = true;
            if (gems.Count >= 3)
            {
                for (int i = 0; i < gems.Count; i++)
                {
                    spawnedGems.Remove(gems[i]);
                    Destroy(gems[i].prefab.transform.parent.gameObject);
                }
                SpawnMinion(gems[0].type);
                pling.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                pling.PlayOneShot(plingClip);
            }
        }
    }

    public void SpawnMinion(GemType type)
    {
        Debug.Log((int)type);
        pwm.Spawn((int)playerId, arrowIndex, (int)type);
    }

    public void SpawnSmallGem()
    {
        int angleRand = rand.Next(0, (int)(360 / angle));
        int gemRand = rand.Next(0, gems.Count);
        GameObject parent = new GameObject();
        parent.transform.SetParent(smallGemParent, false);
        GameObject GO = Instantiate(gems[gemRand].prefab);
        GO.transform.SetParent(parent.transform, false);
        GO.transform.localPosition = new Vector3(0, minDistance + blockHeight * (maxRows * 2.4f));
        //GO.transform.localPosition = new Vector3(Mathf.Cos((angleRand * angle) * Mathf.Deg2Rad), Mathf.Sin((angleRand * angle) * Mathf.Deg2Rad)) * (minDistance + blockHeight * (maxRows - 1));
        parent.transform.rotation = Quaternion.AngleAxis(angleRand * angle - 90, new Vector3(0, 0, 1));
        tk2dSlicedSprite sp = GO.GetComponent<tk2dSlicedSprite>();

        Vector2 v = sp.dimensions;
        v.x = maxGemWidth;
        sp.dimensions = v;
        SpawnGem s = new SpawnGem();
        s.sprite = sp;
        s.prefab = GO;
        s.col = GO.GetComponent<BoxCollider2D>();
        s.type = gems[gemRand].type;
        s.targetAngle = angleRand * angle - 90;
        var i = ((angleRand + (int)((360 - transform.eulerAngles.z) / angle)) + 1) % (int)(360 / angle);
        spawnedGems.Add(s);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Test");
        health -= damage;
        if (health <= 0)
        {
            Lose();
            return;
        }
        int i = Mathf.RoundToInt(health / (100 / healthSprites.Length));
        GetComponent<SpriteRenderer>().sprite = healthSprites[i];
    }

    public void Lose()
    {
        deathScreen.SetActive(true);
        paused = true;
        FindObjectsOfType<Gem>().ToList().ForEach(x => x.paused = true);
        FindObjectsOfType<PixelMonster>().ToList().ForEach(x => x.paused = true);
    }

    public void SpawnCockblocker()
    {
        int angleRand = rand.Next(0, (int)(360 / angle));
        GameObject parent = new GameObject();
        parent.transform.SetParent(smallGemParent, false);
        GameObject GO = Instantiate(cockBlockerPrefab);
        GO.transform.SetParent(parent.transform, false);
        GO.transform.localPosition = new Vector3(0, minDistance + blockHeight * (maxRows * 2.4f));
        //GO.transform.localPosition = new Vector3(Mathf.Cos((angleRand * angle) * Mathf.Deg2Rad), Mathf.Sin((angleRand * angle) * Mathf.Deg2Rad)) * (minDistance + blockHeight * (maxRows - 1));
        parent.transform.rotation = Quaternion.AngleAxis(angleRand * angle - 90, new Vector3(0, 0, 1));
        tk2dSlicedSprite sp = GO.GetComponent<tk2dSlicedSprite>();

        Vector2 v = sp.dimensions;
        v.x = maxGemWidth;
        sp.dimensions = v;
        SpawnGem s = new SpawnGem();
        s.sprite = sp;
        s.prefab = GO;
        s.col = GO.GetComponent<BoxCollider2D>();
        s.type = GemType.Cockblocker;
        s.targetAngle = angleRand * angle - 90;
        var i = ((angleRand + (int)((360 - transform.eulerAngles.z) / angle)) + 1) % (int)(360 / angle);
        spawnedGems.Add(s);
    }
}

[System.Serializable]
public class SpawnGem
{
    public GemType type;
    public GameObject prefab;
    public tk2dSlicedSprite sprite;
    public BoxCollider2D col;
    public float targetAngle;
    public bool moving;
}

public enum GemType
{
    Green = 0 , Red = 1, Blue = 2, Cockblocker
}

public enum PlayerId
{
    P1, P2
}