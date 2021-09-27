using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mara.Lib;
public class MaraUnityManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string ori;
    public string Out;
    public string file;
    
    void Start()
    {
        var mainVita = new Unity.Mara.Lib.Platforms.Generic.Main(ori,Out,file);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
