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
    public List<dtaEliment> PosEliment;
    public List<Transform> RowTest;
    public int Xmin, Xmax;
    public int Ymin, Ymax;
    public HashSet<Transform> DectroyEliment = new HashSet<Transform>();
    public List<dtaEliment> Column;

    public Vector3 Offset;
    public GameObject PrefsBoom;

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

        Xmax = TileZone.Max(a => a.x); Xmin = TileZone.Min(a => a.x);
        Ymax = TileZone.Max(a => a.y); Ymin = TileZone.Min(a => a.y);
        Spawn();
    }

    private void Update()
    {
        if (Time.time - late > 1)
        {
            Cherwall(); 
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Collapse();
        }    
    }

    public void Cherwall()
    {
        DectroyEliment.Clear();
        List<dtaEliment> Cwall = new List<dtaEliment>();

        for (float x = Xmin; x < Xmax; x++)
        {
            float targetX = x + 0.5f;
            Cwall = PosEliment
                .Where(a => Mathf.Approximately(a.grapPosl.position.x, targetX))
                .OrderBy(a => a.grapPosl.position.y)
                .ToList();

            if (Cwall.Count == 0) continue;

            List<Transform> _dEliment = new List<Transform>();
            int combin = Cwall[0].tag;    
            _dEliment.Add(Cwall[0].grapPosl);

            for (int i = 1; i < Cwall.Count; i++)
            {
                var V = Cwall[i];
                if (V.tag == combin)
                {
                    _dEliment.Add(V.grapPosl);
                }
                else
                {
                    if (_dEliment.Count >= 3)
                        DectroyEliment.UnionWith(_dEliment);

                    _dEliment.Clear();
                    combin = V.tag;
                    _dEliment.Add(V.grapPosl);
                }
            }
            if (_dEliment.Count >= 3)
                DectroyEliment.UnionWith(_dEliment);
        }
        CherRow();
    }
    public void CherRow()
    {
        List<dtaEliment> Crow = new List<dtaEliment>();
        for(var y = Ymin; y < Ymax; y++)
        {
            Crow = PosEliment
                .Where(a => Mathf.Approximately(a.grapPosl.position.y, (y + 0.5f)))
                .OrderBy(a =>a.grapPosl.position.x)
                .ToList();

            if (Crow.Count == 0) continue;

            List<Transform> _diei = new List<Transform>();
            int comdin = Crow[0].tag;
            _diei.Add(Crow[0].grapPosl);
            for(var i = 1;i < Crow.Count; i++)
            {
                var V = Crow[i];
                if(comdin == V.tag)
                {
                    _diei.Add(V.grapPosl);
                }
                else
                {
                    if (_diei.Count >= 3)
                        DectroyEliment.UnionWith(_diei);
                    _diei.Clear();
                    comdin = V.tag;
                    _diei.Add(V.grapPosl);
                }
            }
        }
        DectroyEl();
    }
    public void DectroyEl()
    {
        if (DectroyEliment.Count == 0) return;

        // 1) —начала Ч данные (пока grapPosl ещЄ жив)
        PosEliment.RemoveAll(e => e.grapPosl == null || DectroyEliment.Contains(e.grapPosl));

        // 2) ѕотом Ч визуал
        foreach (var t in DectroyEliment)
        {
            if (t == null) continue;
            GameObject boom = Instantiate(PrefsBoom);
            boom.transform.position = t.position;
            Destroy(t.gameObject);
        }

        // 3) ќчистить
        DectroyEliment.Clear();
    }
    public void Collapse()
    {
        for (int x = Xmin; x <= Xmax; x++)
        {
            float targetX = x + 0.5f;

            Column = PosEliment
                .Where(e => e.grapPosl != null
                         && Mathf.Approximately(e.grapPosl.position.x, targetX))
                .OrderBy(e => e.grapPosl.position.y)
                .ToList();

            if (Column.Count == 0) continue;

            for (int i = 0; i < Column.Count; i++)
            {
                float newY = Ymin + i + 0.5f;
                var el = Column[i];

                if (Mathf.Approximately(el.grapPosl.position.y, newY))
                    continue;

                el.grapPosl
                    .DOMoveY(newY, 0.25f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() => el.grapPosl.GetComponent<TileControll>().Ineciliztion());
            }
        }
    }

    public void Spawn()
    {
        for (var i = 0; i < TileZone.Count;i++)
        {
            GameObject El = Instantiate(prifabEliment);
            int ElType = Random.Range(0, SpriteEliment.Length);
            PosEliment.Add(new dtaEliment { grapPosl = El.transform, tag = ElType });
            SpriteRenderer ElRend = El.GetComponent<SpriteRenderer>();
            ElRend.sprite = SpriteEliment[ElType];
            El.transform.position = new Vector3(TileZone[i].x+0.5f, TileZone[i].y+0.5f );
            El.transform.DOScale(0.5f, 0.25f).From(0);
            ElRend.DOFade(1, 0.25f).From(0).OnComplete(()=> El.GetComponent<TileControll>().Ineciliztion());
        }
    }
    [System.Serializable]
    public class dtaEliment
    {
        public Transform grapPosl;
        public int tag;
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