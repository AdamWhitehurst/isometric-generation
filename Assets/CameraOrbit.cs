using UnityEngine;

public class CameraOrbit : MonoBehaviour {
    #region Fields

    public float mouseSensitivity = 4f;

    public float scrollSensitivity = 2f;
    public float orbitDamping = 10f;
    public float scrollDamping = 6f;

    public bool cameraDisabled = false;


    protected Transform _camera_transform;

    protected Transform _parent_transform;

    protected Vector3 _localRotation;

    protected float _cameraDistance = 10f;

    #endregion

    #region MonoBehaviour Methods 
    void Start() {
        _camera_transform = this.transform;
        _parent_transform = transform.parent;
    }

    void LateUpdate() {

        if (Input.GetKeyDown(KeyCode.LeftAlt)) {
            cameraDisabled = !cameraDisabled;
        }

        if (!cameraDisabled) {
            // Rotate based on X and Y of Mouse Coordinates
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            if (mouseX != 0 || mouseY != 0) {
                _localRotation.x += mouseX * mouseSensitivity;
                _localRotation.y -= mouseY * mouseSensitivity;

                // Clamp y rotation to horizon and to not flip
                _localRotation.y = Mathf.Clamp(_localRotation.y, 0f, 90f);
            }
            // Scrol input from mouse scroll
            var scrollAmount = Input.GetAxis("Mouse ScrollWheel");
            if (scrollAmount != 0) {
                scrollAmount *= scrollSensitivity * _cameraDistance * 0.3f;
                _cameraDistance += scrollAmount * -1f;
                // _cameraDistance = Mathf.Clamp(_cameraDistance, 1.5f, 100f);
            }
        }

        // Apply Camera transformations
        var rotation = Quaternion.Euler(_localRotation.y, _localRotation.x, 0);
        _parent_transform.rotation = Quaternion.Lerp(_parent_transform.rotation, rotation, Time.deltaTime * orbitDamping);

        if (_camera_transform.localPosition.z != _cameraDistance * -1f) {
            _camera_transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._camera_transform.localPosition.z, _cameraDistance * -1f, Time.deltaTime * scrollDamping));
        }

    }
    #endregion

    #region CameraOrbit Methods

    #endregion
}
