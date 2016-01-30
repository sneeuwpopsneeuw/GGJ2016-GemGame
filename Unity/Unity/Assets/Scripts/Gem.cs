using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Gem : MonoBehaviour
{
    public float angle = 45;
    public float spawnTime;
    public List<SpawnGem> gems = new List<SpawnGem>();
    public GameObject cockBlockerPrefab;
    public Transform smallGemParent;
    public PlayerId playerId;
    public string leftKey = "lk", rightKey = "rk", upKey = "uk", downKey = "dk";
    public float minGemWidth = 231, maxGemWidth = 231;
    public List<SpawnGem> spawnedGems = new List<SpawnGem>();
    private float spawnTimer;
    public float targetAngle;
    private float minDistance = 2.3f;
    private int maxRows = 5;
    private float blockHeight = .8f;
    RaycastHit2D[] hit = new RaycastHit2D[2];

    void Start()
    {
        DOTween.Init();
        smallGemParent.transform.position = transform.position;
        //SpawnSmallGem();
        //SpawnSmallGem();
        //SpawnSmallGem();
        //SpawnSmallGem();
        spawnTimer = spawnTime;
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnTime)
        {
            spawnTimer = 0;
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

        for (int i = 0; i < spawnedGems.Count; i++)
        {
            SpawnGem sg = spawnedGems[i];
            int count = Physics2D.RaycastNonAlloc(sg.prefab.transform.position, -sg.prefab.transform.up, hit, .5f);
            if (count > 1)
            {
            }
            else
            {
                sg.prefab.transform.position += -sg.prefab.transform.up * Time.deltaTime;
            }
            Vector2 v = sg.sprite.dimensions;
            v.x = Mathf.Lerp(minGemWidth, maxGemWidth, (Vector3.Distance(transform.position, sg.prefab.transform.position) - minDistance) / ((minDistance + blockHeight * (maxRows-1)) - minDistance));
            sg.sprite.dimensions = v;
        }
        hit = new RaycastHit2D[2];
    }

    public void RotateGem(float angle)
    {
        //transform.Rotate(0, 0, angle);
        for (int i = 0; i < spawnedGems.Count; i++)
        {
            SpawnGem sg = spawnedGems[i];
            int count = Physics2D.RaycastNonAlloc(sg.prefab.transform.position, -sg.prefab.transform.up, hit, 1f);
            if (count> 1)
            {
                sg.targetAngle += angle;
                sg.prefab.transform.parent.DOLocalRotate(new Vector3(0,0,angle), .2f, RotateMode.LocalAxisAdd);
            }
        }
        targetAngle += angle;
        transform.DOLocalRotate(new Vector3(0, 0, angle), .2f, RotateMode.LocalAxisAdd);

    }

    public void SpawnSmallGem()
    {
        int angleRand = Random.Range(0, spawnedGems.Count);
        int gemRand = Random.Range(0, gems.Count);
        GameObject parent = new GameObject();
        parent.transform.SetParent(smallGemParent, false);
        GameObject GO = Instantiate(gems[gemRand].prefab);
        GO.transform.SetParent(parent.transform, false);
        GO.transform.localPosition = new Vector3(0, minDistance + blockHeight * (maxRows - 1));
        //GO.transform.localPosition = new Vector3(Mathf.Cos((angleRand * angle) * Mathf.Deg2Rad), Mathf.Sin((angleRand * angle) * Mathf.Deg2Rad)) * (minDistance + blockHeight * (maxRows - 1));
        parent.transform.rotation = Quaternion.AngleAxis(angleRand * angle - 90, new Vector3(0, 0, 1));
        tk2dSlicedSprite sp = GO.GetComponent<tk2dSlicedSprite>();

        Vector2 v = sp.dimensions;
        v.x = maxGemWidth;
        sp.dimensions = v;
        SpawnGem s = new SpawnGem();
        s.sprite = sp;
        s.prefab = GO;
        s.type = gems[gemRand].type;
        s.targetAngle = angleRand * angle - 90;
        var i = ((angleRand + (int)((360 - transform.eulerAngles.z) / angle)) + 1) % (int)(360 / angle);
        spawnedGems.Add(s);
    }

    public void TweenSmallGem(SpawnGem GO, tk2dSlicedSprite sp, int currRow, int angleRand)
    {
        if (currRow > 0)
        {
            currRow--;
            Vector3 dir = (GO.prefab.transform.position - transform.position).normalized;
            GO.prefab.transform.DOLocalMove(Vector3.Lerp(dir * minDistance, dir * (minDistance + blockHeight * (maxRows - 1)), 1f / maxRows * (currRow)), 1f).SetEase(Ease.Linear);
            DOTween.To(() => sp.dimensions, x => sp.dimensions = x, new Vector2(Mathf.Lerp(minGemWidth, maxGemWidth, 1f / maxRows * (currRow)), sp.dimensions.y), 1f).SetEase(Ease.Linear).OnComplete(() =>
               {
                   TweenSmallGem(GO, sp, currRow, angleRand);
               });
        }
    }
}

[System.Serializable]
public class SpawnGem
{
    public GemType type;
    public GameObject prefab;
    public tk2dSlicedSprite sprite;
    public float targetAngle;
}

public enum GemType
{
    Red, Green, Blue
}

public enum PlayerId
{
    P1, P2
}