using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mara.Lib;
using Mara.Lib.Platforms.PS3.IO;
using Yarhl.IO;

public class MaraUnityManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string ori;
    public string Out;
    public string file;
    
    void Start()
    {
        var mainVita = new Mara.Lib.Platforms.Generic.Main(ori,Out,file);
        EDAT_Data A = new EDAT_Data();
        EDAT.validateNPD("", new byte[16], new DataReader(new MemoryStream()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
