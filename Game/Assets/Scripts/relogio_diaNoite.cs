using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class relogio_diaNoite : MonoBehaviour
{

    [SerializeField] private float CicloPorMin = 1f;
   
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, (360 / (CicloPorMin * 60) * Time.deltaTime), Space.Self);
    }
}
