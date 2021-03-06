﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditor : EditorWindow
{
    static MapDataManager data;
    BrushType brushNow = BrushType.WALL;
    Vector2Int tmpMapSize;
    Vector2 scrollPos;
    Rect[,] rects;
    Rect preMouseRect;

    readonly float boxSize = 25;
    readonly Vector2 scrollStartPos = new Vector2(30, 120);
    readonly Vector2 scrollArea = new Vector2(400, 250);
    readonly Vector2 gridStartPos = new Vector2(55, 120);
    readonly Rect gridRect = new Rect(30 + 25, 120 + 25, 400 - 65, 250 - 65);
    readonly Vector2 underScrollArea = new Vector2(0, 400);
    readonly Color[] colors = {
        Color.white,
        new Color(0.5f, 0.5f, 1f, 1),//blue
        new Color(1f, 0.5f, 0.5f, 1),//red
        new Color(0.5f, 1, 0.5f, 1),//green
        new Color(0.5f, 0.5f, 0f, 1)//yellow
    };
        
    enum BrushType { NONE = 0, WALL = 1, PLAYERSPAWN = 3, ENEMYSPAWN = 2, GUN = 4}

    [MenuItem("Window/Map Editor Window")]
    static void Init()
    {
        data = new MapDataManager();
        MapEditor window = GetWindow<MapEditor>("Map Editor");
        window.Show();
    }

    private void OnGUI()
    {
        if (data == null)
            return;

        //맵 사이즈와 Generate part
        EditorGUILayout.BeginHorizontal();
        {
            tmpMapSize = EditorGUILayout.Vector2IntField("Size of Map", tmpMapSize, GUILayout.Width(300));
            tmpMapSize.x = Mathf.Clamp(tmpMapSize.x, 0, 50);
            tmpMapSize.y = Mathf.Clamp(tmpMapSize.y, 0, 50);
            if (GUILayout.Button("Generate", GUILayout.Width(100))){
                GenerateNewMap();
            }
        }
        EditorGUILayout.EndHorizontal();

        //데이터에 맞는 맵 그리기
        DrawMap();

        //마우스의 입력 처리
        if (Event.current.type == EventType.MouseDown)
        {
            ClickEvent(GetCoordWithPosition(Event.current.mousePosition));
        }

        //브러쉬 선택하는 툴바 버튼
        EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
        {
            brushNow = (BrushType)GUILayout.Toolbar((int)brushNow - 1,
                new string[] {
                    BrushType.WALL.ToString(),
                    BrushType.ENEMYSPAWN.ToString(),
                    BrushType.PLAYERSPAWN.ToString(),
                    BrushType.GUN.ToString()
                }) + 1;
        }
        EditorGUILayout.EndHorizontal();

        //등장할 적의 수
        EditorGUILayout.BeginHorizontal();
        {
            int tmpNumberOfEnemy = EditorGUILayout.IntField("Number of Enemy", data.source.numberOfEnemy, GUILayout.Width(300));
            if(tmpNumberOfEnemy != data.source.numberOfEnemy)
                data.SetNumberOfEnemy(tmpNumberOfEnemy);
        }
        EditorGUILayout.EndHorizontal();

        //브러쉬에 따른 박스의 색 미리보기
        EditorGUILayout.BeginHorizontal();
        {
            DrawBoxPreview();
        }
        EditorGUILayout.EndHorizontal();

        //맵의 저장과 불러오기 버튼
        EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
        {
            if (GUI.Button(new Rect(underScrollArea, new Vector2(150, EditorGUIUtility.singleLineHeight)), "Load Map"))
            {
                string path = EditorUtility.OpenFilePanel("Open Map Data", "", "xml");
                if (path.Length != 0)
                {
                    Debug.Log("Load Path >>" + path);
                    LoadMap(path);
                }
            }

            Rect r = new Rect(underScrollArea, new Vector2(150, EditorGUIUtility.singleLineHeight));
            r.position = new Vector2(r.position.x + 150 + 30, r.position.y);

            if(GUI.Button(r, "Save Map"))
            {
                string path = EditorUtility.SaveFilePanel("Save Map Data", "", "New Map","xml");
                if (path.Length != 0)
                {
                    Debug.Log("Save Map Data At >>" + path);
                    SaveMap(path);
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawBoxPreview()
    {
        Color originC = GUI.backgroundColor;
        GUI.backgroundColor = colors[(int)BrushType.WALL];
        GUILayout.Box("", GUILayout.Width(30));
        GUILayout.Label("Wall", GUILayout.Width(50));
        GUI.backgroundColor = colors[(int)BrushType.ENEMYSPAWN];
        GUILayout.Box("", GUILayout.Width(30));
        GUILayout.Label("Enemy", GUILayout.Width(50));
        GUI.backgroundColor = colors[(int)BrushType.PLAYERSPAWN];
        GUILayout.Box("", GUILayout.Width(30));
        GUILayout.Label("Player", GUILayout.Width(50));
        GUI.backgroundColor = colors[(int)BrushType.GUN];
        GUILayout.Box("", GUILayout.Width(30));
        GUILayout.Label("Gun", GUILayout.Width(50));
        GUI.backgroundColor = originC;
    }

    void GenerateNewMap()
    {
        data.ClearMapData();
        data.SetMapSize(tmpMapSize);
        GenerateGridRects();
    }

    void LoadMap(string path)
    {
        data.LoadMapData(path);
        tmpMapSize = data.source.mapSize;
        GenerateGridRects();
    }

    void SaveMap(string path)
    {
        data.SaveMapData(path);
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    void GenerateGridRects()
    {
        rects = new Rect[data.source.mapSize.x, data.source.mapSize.y];

        for (int y = 0; y < data.source.mapSize.y; y++)
        {
            for (int x = 0; x < data.source.mapSize.x; x++)
            {
                rects[x, y] = new Rect(
                    boxSize * x + boxSize, (data.source.mapSize.y * boxSize - boxSize * y),
                    boxSize, boxSize);
            }
        }
    }

    void DrawMap()
    {
        if (rects == null)
            return;
        if (rects.Length == 0)
            return;

        scrollPos = GUI.BeginScrollView(new Rect(scrollStartPos, scrollArea),
            scrollPos,
            new Rect(0, 0, data.source.mapSize.x * boxSize + 50, data.source.mapSize.y * boxSize + 50));

        for(int y = 0; y < data.source.mapSize.y; y++)
        {
            for(int x = 0; x < data.source.mapSize.x; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                string s = x + "," + y;
                BrushType c = BrushType.NONE;
                if (data.source.playerSpawn == coord)
                {
                    s = "P";
                    c = BrushType.PLAYERSPAWN;
                }
                if (data.source.enemySpawns.Contains(coord))
                {
                    s = "E";
                    c = BrushType.ENEMYSPAWN;
                }
                if (data.source.gunSpawn.Contains(coord))
                {
                    s = "G";
                    c = BrushType.GUN;
                }
                if (data.source.walls.Contains(coord))
                {
                    s = "W";
                    c = BrushType.WALL;
                }
                DrawColorBox(rects[x, y], s, c);

            }
        }
        GUI.EndScrollView();
    }
    
    Vector2Int GetCoordWithPosition(Vector2 pos)
    {
        if (gridRect.Contains(pos) == false)
            return new Vector2Int(-1, -1);
        Vector2 result = pos;
        result += scrollPos;
        result -= gridStartPos;
        result.x = Mathf.FloorToInt(result.x / boxSize);
        result.y = data.source.mapSize.y - Mathf.FloorToInt(result.y / boxSize);
        return new Vector2Int((int)result.x, (int)result.y);
    }
    
    void DrawColorBox(Rect r, string s, BrushType boxType)
    {
        GUIContent content = new GUIContent(s);
        DrawColorBox(r, content, colors[(int)boxType]);
    }

    void DrawColorBox(Rect r, GUIContent content, Color c)
    {
        Color originC = GUI.backgroundColor;
        GUI.backgroundColor = c;
        GUI.Box(r, content);
        GUI.backgroundColor = originC;
    }
    
    void SetWall(Vector2Int pos)
    {
        data.SetWall(pos);
        return;
    }

    void SetEnemySpawn(Vector2Int pos)
    {
        data.SetEnemySpawn(pos);

    }

    void SetPlayerSpawn(Vector2Int pos)
    {
        data.SetPlayerSpawn(pos);            
    }

    void SetPistolSpawn(Vector2Int pos)
    {
        data.SetGunSpawn(pos);
    }

    void ClickEvent(Vector2Int pos)
    {
        switch (brushNow)
        {
            case BrushType.WALL:
                SetWall(pos);
                break;
            case BrushType.ENEMYSPAWN:
                SetEnemySpawn(pos);
                break;
            case BrushType.PLAYERSPAWN:
                SetPlayerSpawn(pos);
                break;
            case BrushType.GUN:
                SetPistolSpawn(pos);
                break;                
        }
        
        this.Repaint();
    }

}