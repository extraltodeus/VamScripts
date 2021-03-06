﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace HSTA
{
    public class ReHomePerson : MVRScript
    {

        bool _initialized = false;
        FreeControllerV3 _control;
        List<FreeControllerV3> _frozenControls;
        Coroutine _routine;


        public override void Init()
        {
            try
            {
                if (containingAtom.type != "Person")
                {
                    SuperController.LogError("Use this plugin on a Person only");
                }
                else
                {
                    _control = containingAtom.mainController;
                    _frozenControls = new List<FreeControllerV3>();
                    FreeControllerV3[] controls = containingAtom.freeControllers;
                    foreach (var control in controls)
                    {
                        if (control != _control)
                        {
                            _frozenControls.Add(control);
                        }
                    }

                    _routine = StartCoroutine(FreezeControlsRoutine());
                    _initialized = true;
                }
            }
            catch (Exception e)
            {
                SuperController.LogError("Exception caught: " + e);
            }
        }

        private IEnumerator FreezeControlsRoutine()
        {
            Vector3 lastPos = _control.transform.position;
            Quaternion lastRot = _control.transform.rotation;
            List<Vector3> frozenPositions = new List<Vector3>();
            List<Quaternion> frozenRotations = new List<Quaternion>();
            foreach (var control in _frozenControls)
            {
                frozenPositions.Add(control.transform.position);
                frozenRotations.Add(control.transform.rotation);
            }


            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (lastPos != _control.transform.position || lastRot != _control.transform.rotation )
                {
                    for (int i = 0; i < _frozenControls.Count; ++i)
                    {
                        _frozenControls[i].transform.position = frozenPositions[i];
                        _frozenControls[i].transform.rotation = frozenRotations[i];
                    }
                    lastPos = _control.transform.position;
                    lastRot = _control.transform.rotation;
                }
            }
        }

        void OnEnable()
        {
            if (_initialized)
            {
                _routine = StartCoroutine(FreezeControlsRoutine());
            }
        }

        void OnDisable()
        {
            StopCoroutine(_routine);
        }
    }
}