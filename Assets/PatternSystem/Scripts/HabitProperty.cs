using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class Triger
    {
        string _key;
        public string Key { get { return _key; } }
        protected List<Property> _conditions = new List<Property>();
        public List<Property> Conditions { get { return _conditions; } }

        public Triger(string key, GameObject target)
        {
            key = _key;
        }

        public Triger(string key, GameObject target, Property p)
        {
            key = _key;
            _conditions.Add(p);
        }

        public void Run()
        {
            foreach(Property p in Conditions)
            {
                p.Run();
            }
        }
    }

    public abstract class Property
    {
        protected bool _isDone = false;
        public bool IsDone { get { return _isDone; } }
        protected Property() { }
        public abstract void Run();
        public virtual void Reset(bool isPure)
        {
            _isDone = false;
        }
    }

    public class Arrange : Property
    {
        public enum ArrangeType
        {
            SERIES,
            PARALLEL,
        }

        private ArrangeType _type = ArrangeType.SERIES;
        private List<Property> _properties = new List<Property>();
        private int _curProerty = 0;
        private int _repeatCount;
        private int _curCount = 0;

        public override void Run()
        {
            if (!IsDone)
            {   
                switch (_type)
                {
                    case ArrangeType.SERIES:
                        SeiesRun();
                        break;
                    case ArrangeType.PARALLEL:
                        ParallelRun();
                        break;
                }
                ResetCheck();
            }
        }

        public override void Reset(bool isPure)
        {
            _curProerty = 0;
            _isDone = false;
            _curCount = 0;
            for (int i = 0; i < _properties.Count; ++i)
                _properties[i].Reset(isPure);
        }

        private void SeiesRun()
        {
            _properties[_curProerty].Run();

            if (_properties[_curProerty].IsDone)
                ++_curProerty;
        }

        private void ParallelRun()
        {
            int doneCnt = 0;

            for (int i = 0; i < _properties.Count; ++i)
            {
                Property inter = _properties[i];
                inter.Run();

                if (inter.IsDone)
                    ++doneCnt;
            }
        }
        private void ResetCheck()
        {
            if (_properties.Count == _curProerty)
            {
                if (_repeatCount == 0)
                {
                    for (int i = 0; i < _properties.Count; ++i)
                    {
                        Property p = _properties[i];
                        p.Reset(false);
                        _curProerty = 0;
                        return;
                    }
                }
                else
                {
                    if (_repeatCount == _curCount + 1)
                    {
                        for (int i = 0; i < _properties.Count; ++i)
                        {
                            Property p = _properties[i];
                            p.Reset(true);
                            _curProerty = 0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _properties.Count; ++i)
                        {
                            Property p = _properties[i];
                            p.Reset(false);
                            _curProerty = 0;
                        }
                        ++_curCount;
                    }
                }
                base.Reset(true);
            }
        }
    }

    public class Caller : Property
    {
        public delegate void CallFunc();
        public CallFunc _callFunc;

        public override void Run()
        {
            if (_isDone)
                return;

            if (_callFunc != null)
                _callFunc();

            _isDone = true;
        }
    }

    public class Timer : Property
    {
        float _time;
        public float Time { get { return _time; } }
        float _curTime;
        
        public override void Run()
        {
            if (_isDone)
                return;

            _curTime += UnityEngine.Time.deltaTime;

            if(_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            Reset(isPure);
            if (_curTime > _time && isPure != false)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }

    public abstract class Physical : Property
    {
        protected GameObject _target;
        protected Vector3 _originLocalPoint;
        public Vector3 OriginLocalPosition { get { return _originLocalPoint; } }

        protected Vector3 _translatePoint;
        public Vector3 TranslatePoint { get { return _translatePoint; } }
        protected float _time;
        public float Time { get { return _time; } }
        protected float _curTime = 0.0f;
    }

    public class Move : Physical
    {   
        public override void Run()
        {
            if (_isDone)
                return;
            _curTime += UnityEngine.Time.deltaTime;

            float tickTime = _curTime > _time ? _curTime - _time : UnityEngine.Time.deltaTime;

            if (_time != 0.0f)
                tickTime *= 1 / _time;
            _target.transform.position += _translatePoint * tickTime;

            if (_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time && isPure != false)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }

    public class Rotation : Physical
    {
        public override void Run()
        {
            if (_isDone)
                return;

            _curTime += UnityEngine.Time.deltaTime;

            float tickTime = _curTime > _time ? _curTime - _time : UnityEngine.Time.deltaTime;
            
            if (_time != 0.0f)
                tickTime *= 1 / _time;

            Vector3 rotate = new Vector3(tickTime * _translatePoint.x
                                    , tickTime * _translatePoint.y
                                    , tickTime * _translatePoint.z);

            _target.transform.Rotate(rotate, Space.World);

            if (_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time && isPure != false)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }

    public class Orbit : Physical
    {
        public override void Run()
        {
            if (_isDone)
                return;

            _curTime += UnityEngine.Time.deltaTime;

            float tickTime = _curTime > _time ? _curTime - _time : UnityEngine.Time.deltaTime;

            if (_time != 0.0f)
                tickTime *= 1 / _time;

            Quaternion localRotation = _target.transform.localRotation;
            _target.transform.RotateAround(_originLocalPoint, Vector3.right, _translatePoint.x * tickTime);
            _target.transform.RotateAround(_originLocalPoint, Vector3.up, _translatePoint.y * tickTime);
            _target.transform.RotateAround(_originLocalPoint, Vector3.forward, _translatePoint.z * tickTime);
            _target.transform.localRotation = localRotation;

            if (_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time && isPure != false)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }

    }

    public class Scale : Physical
    {
        public override void Run()
        {
            if (_isDone)
                return;

            if (_time == 0.0f)
                return;

            Transform t = _target.transform.Find("Char(Clone)");
            Transform p = null;
            if (t != null)
            {
                p = t.parent;
                t.SetParent(null);
            }

            Vector3 _scaleOff = new Vector3();

            if (_curTime == 0.0f)
            {
                _originLocalPoint = _target.transform.localScale;
                Vector3 resultScale = new Vector3(_originLocalPoint.x * _translatePoint.x, _originLocalPoint.y * _translatePoint.y, _originLocalPoint.z * _translatePoint.z);

                if (resultScale.x < _originLocalPoint.x)
                {
                    _scaleOff.x = -resultScale.x;
                    _scaleOff.y = -resultScale.y;
                    _scaleOff.z = -resultScale.z;
                }
                else
                {
                    _scaleOff.x = resultScale.x - _originLocalPoint.x;
                    _scaleOff.y = resultScale.y - _originLocalPoint.y;
                    _scaleOff.z = resultScale.z - _originLocalPoint.z;
                }
            }

            _curTime += UnityEngine.Time.deltaTime;
            Vector3 localSale = new Vector3((_originLocalPoint.x + (_scaleOff.x * _curTime / _time))
                                            , (_originLocalPoint.y + (_scaleOff.y * _curTime / _time))
                                            , (_originLocalPoint.z + (_scaleOff.z * _curTime / _time)));
            _target.transform.localScale = localSale;

            if (_curTime >= _time)
            {
                _isDone = true;
            }

            if (t != null)
            {
                t.SetParent(p, true);
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }
}

