using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockSystem : MonoBehaviour
{
    public List<LockPiece> lockPieces = new List<LockPiece>();
    public float rotateSpeed = 180f;
    private int currentStep = 0;
    private bool isRotating = false;

    private void Awake()
    {
        foreach (var piece in lockPieces)
        {
            piece.system = this;
        }
    }
    public void TryUnlock(LockPiece piece)
    {
        if (isRotating)
        {
            Debug.Log("LockSystem: currently rotating. Wait.");
            return;
        }

        if (currentStep >= lockPieces.Count)
        {
            Debug.Log("LockSystem: all locks solved.");
            return;
        }

        if (lockPieces[currentStep] != piece)
        {
            Debug.Log($"LockSystem: Wrong lock clicked. Expected '{lockPieces[currentStep].name}', got '{piece.name}'.");
            return;
        }

        if (piece.solved)
        {
            Debug.Log($"LockSystem: '{piece.name}' already solved.");
            return;
        }

        StartCoroutine(RotateAndActivate(piece));
    }

    private IEnumerator RotateAndActivate(LockPiece piece)
    {
        isRotating = true;

        Transform t = piece.transform;
        Quaternion start = t.localRotation;
        Quaternion end = Quaternion.Euler(piece.targetLocalEuler);

        float angle = Quaternion.Angle(start, end);
        float duration = Mathf.Max(0.0001f, angle / rotateSpeed);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float frac = Mathf.Clamp01(elapsed / duration);
            t.localRotation = Quaternion.Slerp(start, end, frac);
            yield return null;
        }

        t.localRotation = end;

        if (piece.cardObject != null)
            piece.cardObject.SetActive(true);

        piece.solved = true;
        currentStep++;
        isRotating = false;

        Debug.Log($"LockSystem: solved '{piece.name}' ({currentStep}/{lockPieces.Count}).");
    }
}
