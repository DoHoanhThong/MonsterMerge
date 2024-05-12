using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;


[System.Serializable]
public class Datalist
{
    public int id;
    public List<int> _listIDSave = new List<int>();
}
public class DATAManager : MonoBehaviour
{
    private string filename = "/filesave.dat";
    public string path;
    public Datalist _listDATA;
    void Start()
    {
        if (_listDATA == null)
            _listDATA = new Datalist();
        if (_listDATA._listIDSave == null)
            _listDATA._listIDSave = new List<int>();
        path =Application.dataPath + filename;
    } 
    public void SAVECLICK()
    {
        if (!File.Exists(path))
        {
            File.Create(path);
            StreamWriter sw = new StreamWriter(path, true);// file k ton tai thi ghi de len
            WriteData(sw);
        }
        else
        {
            StreamWriter sw = new StreamWriter(path, false);
            WriteData(sw);
        }
    }
    public void WriteData(StreamWriter sw)
    {
        sw.WriteLine(_listDATA.id);
        _listDATA._listIDSave.Add(_listDATA.id);
    }
}
