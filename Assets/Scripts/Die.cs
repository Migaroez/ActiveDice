using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Die : MonoBehaviour
{
    [System.Serializable]
    public class DieSide
    {
        public int Value;
        public Vector3 Normal;
    }
    public List<DieSide> Sides;

    private Rigidbody _rigidBody;
    private bool _isRolling;
    private Vector3 _lastPosition;
    [SerializeField] private float _stoppedMovingTolerance = 0.001f;
    [SerializeField] private float _timeTolerance = 0.5f;
    private float _timeToleranceTimer;

    public int CachedValue { get; private set; }

    public event EventHandler StoppedRolling;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRolling) CheckStoppedRolling();
    }

    public void Roll()
    {
        if (_isRolling) return;

        _lastPosition = transform.position;
        transform.Translate(Vector3.up *5,Space.World);
        _rigidBody.AddForce(GetRandomForceVector());
        _rigidBody.AddTorque(GetRandomTorqueVector());
        _isRolling = true;
    }

    private void CheckStoppedRolling()
    {
        if (Vector3.Distance(transform.position, _lastPosition) > _stoppedMovingTolerance)
        {
            _lastPosition = transform.position;
            _timeToleranceTimer = 0f;
            return;
        }

        _timeToleranceTimer += Time.deltaTime;
        if (_timeToleranceTimer < _timeTolerance) return;

        _isRolling = false;
        StoppedRolling?.Invoke(this,null);
    }

    
    public DieSide GetCurrentSide()
    {
        if (Sides == null || Sides.Count == 0)
            return null;
        var up = transform.InverseTransformDirection(Vector3.up);
        DieSide returnValue = null;
        var angle = float.MaxValue;
        foreach (var side in Sides)
        {
            var sideAngle = Vector3.Angle(side.Normal, up);
            if (sideAngle < angle)
            {
                angle = sideAngle;
                returnValue = side;
            }
        }
        return returnValue;
    }

    public int GetCurrentValue()
    {
        var side = GetCurrentSide();
        CachedValue = side?.Value ?? 0;
        return CachedValue;
    }

    private Vector3 GetRandomForceVector()
    {
        return new Vector3(GetRandomForceValue(), GetRandomForceValue(), GetRandomForceValue());
    }

    private float GetRandomForceValue()
    {
        return Random.Range(500f, 700f) * (Random.Range(0,2) == 0 ? -1 : 1);
    }

    private Vector3 GetRandomTorqueVector()
    {
        return new Vector3(GetRandomTorqueValue(), GetRandomTorqueValue(), GetRandomTorqueValue());
    }

    private float GetRandomTorqueValue()
    {
        return Random.Range(1000f, 3000f) * (Random.Range(0, 2) == 0 ? -1 : 1);
    }
}
