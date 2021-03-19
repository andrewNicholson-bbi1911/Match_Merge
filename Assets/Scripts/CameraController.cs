using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public List<Transform> objectsInCamera;

    private Transform _selfTransform;

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private RectTransform _panelOfView;

    private static float _cameraWidth;
    private static float _cameraHeight;

    private Vector3 _parentCenterInPixCoord;
    [SerializeField] private float _borderInProportion = 0.08f;
    [SerializeField] private float _minDistance;
    [SerializeField] private float _maxDistance;

    [SerializeField] private bool _isDynamicUpdate;

    private float _topBorder;
    private float _bottomBorder;
    private float _rightBorder;
    private float _leftBorder;

    private float _pxlDstFrmCntrTo_Top_Brdr;
    private float _pxlDstFrmCntrTo_Bottom_Brdr;
    private float _pxlDstFrmCntrTo_Right_Brdr;
    private float _pxlDstFrmCntrTo_Left_Brdr;

    private void OnEnable()
    {
        InitializeDefaults();
        ChangeCameraDistance(true);
    }

    internal void InitializeDefaults()
    {
        _selfTransform = transform;
        _cameraWidth = _camera.pixelWidth;
        _cameraHeight = _camera.pixelHeight;
        _parentCenterInPixCoord = _camera.WorldToScreenPoint(_selfTransform.position);
        var border = Mathf.Min(_cameraHeight * _borderInProportion, _cameraWidth * _borderInProportion);
        _bottomBorder = _leftBorder = border;
        _rightBorder = _cameraWidth - border;
        _topBorder = _cameraHeight - border;
        //_cameraTransform = _camera.transform;

        _pxlDstFrmCntrTo_Right_Brdr = _rightBorder - _parentCenterInPixCoord.x;
        _pxlDstFrmCntrTo_Left_Brdr = _leftBorder - _parentCenterInPixCoord.x;
        _pxlDstFrmCntrTo_Top_Brdr = _topBorder - _parentCenterInPixCoord.y;
        _pxlDstFrmCntrTo_Bottom_Brdr = _bottomBorder - _parentCenterInPixCoord.y;
    }


    void Update()
    {
        if (_isDynamicUpdate)
        {
            UpdateCamera();
        }
    }

    public void UpdateCamera()
    {

    }

    private Vector2 _xVector = Vector2.right;
    private Vector2 _yVector = Vector2.up;
    
    /// <summary>
    /// sets x and y axis of view derected to parent canvas
    /// </summary>
    private void SetAxisOfView()
    {
        _xVector = new Vector2( Mathf.Cos(_panelOfView.localRotation.eulerAngles.z * Mathf.Rad2Deg)
            , Mathf.Sin(_panelOfView.localRotation.eulerAngles.z * Mathf.Rad2Deg)) * _panelOfView.rect.width;
        _yVector = Vector2.Perpendicular(_xVector).normalized * _panelOfView.rect.height;
    }

    /// <summary>
    /// returns 2 points (left-bottom and right-top) of viewpanel in worldspace rectangle that is include all the objects should be inside of view panel   
    /// </summary>
    /// <returns></returns>
    private Vector3[] Vectors_()
    {
        return null;
    }
    internal void ChangeCameraDistance( bool momentous = false)
    {
        var inCam = _camera.WorldToScreenPoint(_target.position);
        float x;
        if (inCam.x > _parentCenterInPixCoord.x)
        {
            x = (inCam.x - _parentCenterInPixCoord.x) / _pxlDstFrmCntrTo_Right_Brdr;
        }
        else
        {
            x = (inCam.x - _parentCenterInPixCoord.x) / _pxlDstFrmCntrTo_Left_Brdr;

        }

        float y;
        if (inCam.y > _parentCenterInPixCoord.y)
        {
            y = (inCam.y - _parentCenterInPixCoord.y) / _pxlDstFrmCntrTo_Top_Brdr;
        }
        else
        {
            y = (inCam.y - _parentCenterInPixCoord.y) / _pxlDstFrmCntrTo_Bottom_Brdr;
        }

        var newPosition = _cameraTransform.localPosition * (Mathf.Max(x, y));
        if (newPosition.magnitude == 0)
            return;
        if(!momentous)
            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, newPosition, 2 * Time.deltaTime);
        else
        {
            _cameraTransform.localPosition = newPosition;

        }

        if (_cameraTransform.localPosition.magnitude < _minDistance)
        {
            _cameraTransform.localPosition = _cameraTransform.localPosition.normalized * _minDistance;
        }
        else if (_cameraTransform.localPosition.magnitude > _maxDistance)
        {
            _cameraTransform.localPosition = _cameraTransform.localPosition.normalized * _maxDistance;
        }

    }

    /// <summary>
    /// returns random Vector3 point in WorldPosition that is out of Camera FieldView;
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRandomPointOutOfCamera()
    {
        if (_cameraHeight == 0)
        {
            InitializeDefaults();
        }
        var angle = Random.Range(0f, 360f);
        Vector3 a;
        if (angle <= 30)
        {
            a = new Vector3(angle / 30f * _cameraWidth, _cameraHeight * 1.1f, 80);
        }
        else if (angle <= 60)
        {
            a = new Vector3((angle - 30) / 30f * _cameraWidth, -_cameraHeight * 0.1f, 80);
        }
        if (angle <= 210)
        {
            a = new Vector3(_cameraWidth * 1.1f, (angle - 60) / 150f * _cameraHeight, 80);
        }
        else
        {
            a = new Vector3(-_cameraWidth * 0.1f, (angle - 210) / 150f * _cameraHeight, 80);
        }
        return _camera.ScreenToWorldPoint(a);
    }
}
