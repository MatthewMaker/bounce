﻿using UnityEngine;
using UniRx;

namespace Com.Larkintuckerllc.Bounce
{
    public class MainCamera : MonoBehaviour
    {
        bool _casting = false;
        float _last = 0.0f;
        Mode.ModeEnum _mode = Mode.InitialState;
        bool _placementValid = PlacementValid.InitialState;

        void Start()
        {
            Provider.Store.Subscribe(state =>
            {
                Mode.ModeEnum nextMode = Mode.ModeGet(state);
                bool nextPlacementValid = PlacementValid.PlacementValidGet(state);
                if (nextMode == _mode && nextPlacementValid == _placementValid) { return; }
                _mode = nextMode;
                _placementValid = nextPlacementValid;
                _casting = (_mode == Mode.ModeEnum.Placement);
            });
        }

        void Update()
        {
            var now = Time.time;
            if (_casting && ((now - _last) >= 1.0f)) {
                Cast();
            }
        }

		void Cast()
        {
            _last = Time.time;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                var placementX = (int)(hit.point.x * 100);
                var placementY = (int)(hit.point.y * 100);
                var placementZ = (int)(hit.point.z * 100);
                var placement = new Triplet(placementX, placementY, placementZ);
                Provider.Dispatch(Placement.Instance.PlacementSet(placement));
                if (!_placementValid)
                {
                    Provider.Dispatch(PlacementValid.Instance.PlacementValidSet(true));
                }
            }
        }
    }
}