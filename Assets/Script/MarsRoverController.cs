using UnityEngine;

public class MarsRoverController : MonoBehaviour
{
    private void Awake()
    {
        transform.position = new Vector3((float)-6.223, (float)-4.528, 0);
    }
    string[] order = { "LMLMLMLMM" };
    
    
}