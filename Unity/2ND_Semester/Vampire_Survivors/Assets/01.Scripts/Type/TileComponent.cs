using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileComponent : MonoBehaviour, Icomponent
{
    [SerializeField] private Tilemap[] tiles;
    [SerializeField] private TileBase[] tilebases;

    private int size = 64;

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.INIT:
                Init();
                break;
        }
    }

    private void Init()
    {
        GameManager.Instance.GetGameComponent<ChunkComponent>().ChunkCreateSubscribe(ChunkGenerateEvent);
        GameManager.Instance.GetGameComponent<ChunkComponent>().ChunkRemoveSubscribe(ChunkRemoveEvent);
    }

    private void ChunkRemoveEvent(Chunk chunk)
    {
        var chunkSize = chunk.map.GetLength(0);

        var start = chunk.index * (chunkSize - 1);
        start += new Vector3Int(chunk.index.x - (int)(chunkSize * 0.5f), chunk.index.y - (int)(chunkSize * 0.5f));

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                var tilePosition = new Vector3Int(start.x + i, start.y + j);

                for (int z = 0; z < tiles.Length; i++)
                    tiles[z].SetTile(tilePosition, null);
            }
        }
    }

    private void ChunkGenerateEvent(Chunk chunk)
    {
        var chunkSize = chunk.map.GetLength(0);

        var start = chunk.index * (chunkSize - 1);
        start += new Vector3Int(chunk.index.x - (int)(chunkSize * 0.5f), chunk.index.y - (int)(chunkSize * 0.5f));

        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                var tilePosition = new Vector3Int(start.x + i, start.y + j);

                tiles[chunk.map[i, j]].SetTile(tilePosition, tilebases[chunk.map[i, j]]);

                tiles[0].SetTile(tilePosition, tilebases[0]);
            }
        }
    }

    private void Reset()
    {
        //tile.ClearAllTiles();
    }
}

