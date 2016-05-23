using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
    public class Habit : MonoBehaviour
    {
        Dictionary<string, Triger> _trigers = new Dictionary<string, Triger>();
        string _curTirger = "";

        public void Play(string trigerKey)
        {
            _curTirger = trigerKey;
        }

        public void AddTriger(Triger t)
        {
            _trigers.Add(t.Key, t);
        }

        // Update is called once per frame
        void Update()
        {
            if(_trigers.ContainsKey(_curTirger))
            {
                _trigers[_curTirger].Run();
            }
        }
    }

}

