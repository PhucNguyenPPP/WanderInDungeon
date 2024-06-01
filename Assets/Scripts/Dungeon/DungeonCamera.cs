using System;
using Cinemachine;
using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
    private CinemachineVirtualCamera cmVC;

    private void Awake()
    {
        cmVC = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        cmVC.Follow = LevelManager.Instance.SelectedPlayer.transform;
    }
}
