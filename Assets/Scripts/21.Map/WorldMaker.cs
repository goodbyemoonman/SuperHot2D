﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldMaker : MonoBehaviour {
    MapDataManager mdm;
    public Tilemap obstacleGrid;
    public Tile obstacle;
    string dataFilePath;

    private void Awake()
    {
        mdm = new MapDataManager();
        dataFilePath = Application.dataPath + "/Resources/MapData/";
    }

    private void OnEnable()
    {
        SetTileMap();
    }

    public void SetTileMap()
    {
        mdm.LoadMapData(dataFilePath + "S1.xml");

        for(int i = 0; i < mdm.source.walls.Count; i++)
        {
            Vector3Int pos = new Vector3Int(
                mdm.source.walls[i].x - mdm.source.playerSpawn.x, 
                mdm.source.walls[i].y - mdm.source.playerSpawn.y, 
                0);
            obstacleGrid.SetTile(pos, obstacle);
        }
    }
}