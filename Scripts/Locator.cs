using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Locator : MonoBehaviour
{
    [Multiline(2)]
    public string UpdateString = "Targets left: {0}\nBumper to skip";
    public float RotationSpeed = 720;
    public GameObject LocatorArrow;
    public Text LocatorText;

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void UpdateLocator(GameObject toTarget, int targetsRemaining)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        var headPos = Camera.main.transform.position;
        var planeNormal = (headPos - LocatorArrow.transform.position).normalized;
        var plane = new Plane(planeNormal, LocatorArrow.transform.position);
        var nearest = plane.ClosestPointOnPlane(toTarget.transform.position);
        var right = (nearest - LocatorArrow.transform.position).normalized;
        var rot = Quaternion.LookRotation(planeNormal, Vector3.Cross(planeNormal, right));
        var currentRot = LocatorArrow.transform.rotation;
        LocatorArrow.transform.rotation = Quaternion.RotateTowards(currentRot, rot, RotationSpeed * Time.deltaTime);
        LocatorText.text = string.Format(UpdateString, targetsRemaining);
    }
}
