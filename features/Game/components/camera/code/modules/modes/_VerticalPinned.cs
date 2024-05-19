using System;
using Godot;

namespace ThousandDevils.features.Game.components.camera.code.modules.modes;

public class VerticalPinnedMode : BaseMode
{
  private readonly float _movementSmoothness = 0.025f;
  private readonly Node3D _rootArm;
  private readonly Node3D _rotationArm;
  private readonly float _rotationSmoothness = 0.03f;
  private readonly float _zoomSmoothness = 0.03f;
  private readonly Vector3 farZoom = new(0, 30, 30);
  private readonly Vector3 nearZoom = new(0, 5, 5);

  //variables for handle lerp
  private Vector3 _movementVelocity;

  private Vector3 _targetAngle;
  private Vector3 _targetPos;
  private Vector3 _targetZoom;
  private float _yRotationAngle;
  private float _zoomDir;

  public VerticalPinnedMode(Camera camera) : base(camera) {
    _rotationArm = Camera.GetParent<Node3D>();
    _rootArm = _rotationArm.GetParent<Node3D>();
    _targetPos = _rootArm.GlobalPosition;
    _targetAngle = _rotationArm.Rotation;
    _targetZoom = Camera.Position;
  }

  //todo В теории можно смещать центр относительно которого вращается камера с помощью рейкаста в середину экрана определенной длины, чтобы на близких расстояниях это работало хорошо. Делать это можно при достижении зума определенных значений.

  public float MovementSpeed { get; set; } = 30;
  public float RotationSpeed { get; set; } = (float)Math.PI / 3.5f;
  public float ZoomSpeed { get; set; } = 30;

  private void CalculateRotationAngle(double delta) {
    _yRotationAngle = Input.GetAxis("cam_arm_rotate_counterclock", "cam_arm_rotate_clock") * RotationSpeed * (float)delta;
  }

  private void HandleRotation() {
    //lower rotation smoothness would be smoother
    if (_yRotationAngle != 0) _targetAngle += new Vector3(_rotationArm.Rotation[0], _yRotationAngle, _rotationArm.Rotation[2]);
    if (_targetAngle != _rotationArm.Rotation) _rotationArm.Rotation = _rotationArm.Rotation.Lerp(_targetAngle, _rotationSmoothness);
  }

  private void CalculateMovement(double delta) {
    Vector2 dir = Input.GetVector("cam_free_right", "cam_free_left", "cam_free_forward", "cam_free_backward");
    Vector3 calculatedDir = dir == Vector2.Zero ? Vector3.Zero : (_rotationArm.Transform.Basis * new Vector3(dir[1], 0, dir[0])).Normalized();
    _movementVelocity = calculatedDir * MovementSpeed * _targetZoom[2] / (farZoom[2] / 1.5f) * (float)delta;
  }

  private void HandleMovement() {
    if (_movementVelocity != Vector3.Zero) _targetPos += _movementVelocity;
    if (_targetPos != _rootArm.GlobalPosition) _rootArm.GlobalPosition = _rootArm.GlobalPosition.Lerp(_targetPos, _movementSmoothness);
  }

  private void CalculateZoom(double delta) {
    _zoomDir = 0;
    if (Input.IsActionJustPressed("wheel_up")) _zoomDir = -1;
    if (Input.IsActionJustPressed("wheel_down")) _zoomDir = 1;
    _zoomDir *= ZoomSpeed * (float)delta;
  }

  private void HandleZoom() {
    if (_targetZoom != Camera.Position) Camera.Position = Camera.Position.Lerp(_targetZoom, _zoomSmoothness);
    if (_zoomDir != 0) {
      Vector3 zoomVector = new(0, _zoomDir, _zoomDir);
      if (_targetZoom + zoomVector >= nearZoom && _targetZoom + zoomVector <= farZoom) _targetZoom += zoomVector;
    }
  }


  public override void OnProcess(double delta) {
    HandleRotation();
    HandleMovement();
    HandleZoom();
  }

  public override void OnPhysicsProcess(double delta) {
    CalculateRotationAngle(delta);
    CalculateMovement(delta);
    CalculateZoom(delta);
  }
}