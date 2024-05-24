using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerManager : MonoBehaviour
{
    private const float OverlapCircleRadius = 0.45f;
    private ContactFilter2D _contactFilter;
    private List<Collider2D> _raycastHits = new List<Collider2D>();
    [SerializeField] public SpawnerInfo si;
    private TileReservationManager _trm;
    private Vector3 _position;
    private List<Monster> _monsterList = new List<Monster>();
    private void Start()
    {
        _trm = TileReservationManager.Instance;
        _position = transform.position;
        SetUpContactFilter();
        StartCoroutine(InitialCoroutine());

    }

    private IEnumerator InitialCoroutine()
    {
        for (int i = 0; i < si.numMonsters; i++)
        {
            StartCoroutine(SpawnRoutine());
            yield return new WaitForSeconds(si.delay);
        }
        yield break;
    }
    
    private void SetUpContactFilter()
    {
        _contactFilter = new ContactFilter2D();
        _contactFilter.layerMask = LayerMask.GetMask("Monsters", "Obstacles", "Player");
        _contactFilter.useLayerMask = true;

    }  

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(si.delay);

        bool isFound = false;

        while (!isFound)
        {
            Vector3 randomPositionInBox = GetRandomPositionInBox(si.boxHalfSide);
            Vector2Int targetReserveTile = Vector2Int.FloorToInt(randomPositionInBox);

            if (!IsCollision(targetReserveTile))
            {
                if (_trm.ReserveTile(targetReserveTile, gameObject))
                {
                    SpawnMonster(Resources.Load<GameObject>(si.monsterPrefabPath), Resources.Load<GameObject>(si.spawnEffectPrefabPath),
                        randomPositionInBox);
                    _trm.ReleaseTile(targetReserveTile, gameObject);
                    isFound = true;
                }
                
            }
        }
        yield break;

    }
    
    public Vector3 GetRandomPositionInBox(int boxHalfSide)
    {
        float randomX = Mathf.Round(Random.Range(_position.x - boxHalfSide, _position.x + boxHalfSide)) + 0.5f;
        float randomY = Mathf.Round(Random.Range(_position.y - boxHalfSide, _position.y + boxHalfSide)) + 0.5f;
        return new Vector3(randomX, randomY, _position.z);
    }
    
    private void SpawnMonster(GameObject monsterPrefab, GameObject spawnEffectPrefab, Vector3 position)
    {
        Instantiate(spawnEffectPrefab, position, Quaternion.identity, transform.parent);
        GameObject monsterObj = Instantiate(monsterPrefab, position, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();
        if (monster != null)
        {
            _monsterList.Add(monster);
            monster.OnMonsterDestroyed += HandleMonsterDestruction;
        }
    }
    private void HandleMonsterDestruction(Monster destroyedMonster)
    {
        _monsterList.Remove(destroyedMonster);
        destroyedMonster.OnMonsterDestroyed -= HandleMonsterDestruction;
        StartCoroutine(SpawnRoutine());
        Debug.Log("Monster destroyed: " + destroyedMonster.name);
    }
    
    private bool IsCollision(Vector2Int targetReserveTile)
    {
        return Physics2D.OverlapCircle(targetReserveTile, OverlapCircleRadius, _contactFilter, _raycastHits) > 0 || _trm.IsTileReserved(targetReserveTile);
    }
}