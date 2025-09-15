using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class LockSystem : MonoBehaviour
{
    public List<LockPiece> lockPieces = new List<LockPiece>();
    public float rotateSpeed = 180f;

    private bool isRotating = false;

    private void Awake()
    {
        foreach (var piece in lockPieces)
            piece.system = this;
    }

    public void TryUnlock(LockPiece piece)
    {
        if (isRotating) return;
        if (piece.solved) return;

        RotateAndActivate(piece);
    }

    private void RotateAndActivate(LockPiece piece)
    {
        isRotating = true;

        Transform t = piece.transform;
        Quaternion start = t.localRotation;
        Quaternion end = Quaternion.Euler(piece.targetLocalEuler);

        float angle = Quaternion.Angle(start, end);
        float duration = Mathf.Max(0.0001f, angle / rotateSpeed);

        t.DOKill();

        t.DOLocalRotate(piece.targetLocalEuler, duration, RotateMode.Fast)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (piece.cardObject != null)
                    piece.cardObject.SetActive(true);

                piece.solved = true;
                isRotating = false;
            });
    }
}