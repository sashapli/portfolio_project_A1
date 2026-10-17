using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TileEliment : MonoBehaviour
{
    public static TileEliment Singalton;

    private float late;
    [SerializeField] private GameObject prifabEliment;
    [SerializeField] private Sprite[] SpriteEliment;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Sprite ZoneEliment;
    [SerializeField] private float MinY = -99;
    [SerializeField] private float OffsetY = 99;
    private int LightPoleY = 0;

    public List<Vector2Int> TileZone;
    public List<Vector2Int> TileZoneSpawn;
    public List<Vector2Int> TileFreeWightY = new List<Vector2Int>();
    public List<Transform> PosEliment;
    public int maxY;

    public Vector3 Offset;


    private void Awake() => Singalton = this;

    private void Start()
    {
        TileZone = new List<Vector2Int>();
        foreach (var p in tilemap.cellBounds.allPositionsWithin)
            if (tilemap.GetSprite(p) == ZoneEliment)
                TileZone.Add((Vector2Int)p);

        TileZoneSpawn = TileZone
            .GroupBy(e => e.x)
            .Select(g => new Vector2Int(g.Key, g.Max(p => p.y)))
            .OrderBy(v => v.x)
            .ToList();

        TileFreeWightY = TileZone
            .GroupBy(pos => pos.x)
            .OrderBy(g => g.Key)
            .Select(g => new Vector2Int(g.Key, g.Count()))
            .ToList();
        //SpawnRow();
    }

    private void Update()
    {
        if(Time.time - late > 1)
        {
            late = Time.time;
            if (TileFreeWightY.Any(e => e.y > 0))
                SpawnRow(); 
        }
    }

    public void SpawnRow()
    {
        for (var i = 0; i < TileFreeWightY.Count;i++)
        {
            if (TileFreeWightY[i].y <= 0) continue;
            TileFreeWightY[i] = new Vector2Int(TileFreeWightY[i].x, TileFreeWightY[i].y - 1);
            GameObject El = Instantiate(prifabEliment);
            PosEliment.Add(El.transform);
            Debug.Log(new Vector2(TileZoneSpawn[i].x, TileZoneSpawn[i].y - TileFreeWightY[i].y));
            int ElType = Random.Range(0, SpriteEliment.Length);
            SpriteRenderer ElRend = El.GetComponent<SpriteRenderer>();
            ElRend.sprite = SpriteEliment[ElType];
            El.transform.position = new Vector3(TileZoneSpawn[i].x+0.5f, TileZoneSpawn[i].y+1);
            El.transform.DOMoveY(TileZoneSpawn[i].y + 0.5f - TileFreeWightY[i].y, (float)((TileFreeWightY[i].y+1f) /5f)).SetEase(Ease.InSine).OnComplete(() => El.GetComponent<TileControll>().Ineciliztion());
            ElRend.DOFade(1, 0.25f).From(0);
        }
    }
    [System.Serializable]
    public class dtaEliment
    {
        public Vector2Int grapPosl;
        public TileControll gl;
        public bool isAct;
    }
}

/*    IEnumerator SpawnEliment()
    {
        while (PosEliment.Exists(e => e.isAct == false ))
        {
            yield return new WaitForSeconds(0.2f);
            foreach (var pos in SpawnPontYEliment)
            {
                //создание элимента
                GameObject El = Instantiate(prifabEliment);
                //сохранение компонента
                var tc = El.GetComponent<TileControll>();
                // назначение спрайта
                El.GetComponent<SpriteRenderer>().sprite = SpriteEliment[Random.Range(0, SpriteEliment.Length)];
                // назначение координат
                El.transform.position = new Vector3(pos.x, pos.y + OffsetY, 0) + Offset;
                List<Vector2Int> keysToSet = new List<Vector2Int>();
                MinY = 99;
                foreach (var i in PosEliment)
                {
                    //Debug.Log("" + i.Key + i.Value);
                    if (!i.isAct)
                    {
                        if (i.grapPosl.x == pos.x)
                        {
                            if (MinY > i.grapPosl.y)
                            {
                                keysToSet.Add(i.grapPosl);
                                MinY = i.grapPosl.y;
                            }
                        }
                    }
                }
                foreach (var i = 0;i < keysToSet.Count;i++)
                {
                    PosEliment[i].isAct = true;
                    PosEliment[i].gl = tc;
                }
                El.transform.DOMove(new Vector3(El.transform.position.x, -Mathf.Abs(MinY - pos.y) + 1.5f, 0), Mathf.Abs(MinY - pos.y) * 0.2f+1).OnComplete(() => El.GetComponent<TileControll>().Ineciliztion());
            }
        }
    }
*/