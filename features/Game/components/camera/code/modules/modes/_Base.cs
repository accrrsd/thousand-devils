using System;
using Godot;
using Godot.Collections;

namespace ThousandDevils.features.Game.components.camera.code.modules.modes;

public class BaseMode
{
  protected readonly Camera Camera;

  protected BaseMode(Camera camera) {
    Camera = camera;
  }

  public bool ProcessDefaultRayCast { get; set; } = true;

  public virtual void OnProcess(double delta) { }
  public virtual void OnInput(InputEvent @event) { }
  public virtual void OnReady() { }

  public event Action<Node> OnRayCast;

  protected Node ShootRay(int rayLength = 1000) {
    Vector2 mousePos = Camera.GetViewport().GetMousePosition();
    Vector3 from = Camera.ProjectRayOrigin(mousePos);
    Vector3 to = from + Camera.ProjectRayNormal(mousePos) * rayLength;
    PhysicsDirectSpaceState3D space = Camera.GetWorld3D().DirectSpaceState;
    PhysicsRayQueryParameters3D rayQuery = new() {
      From = from,
      To = to
    };
    Dictionary res = space.IntersectRay(rayQuery);
    if (res.Count == 0 || !res.ContainsKey("collider")) return null;
    StaticBody3D staticBody = (StaticBody3D)res["collider"];
    Node node = staticBody.GetParent();
    if (node is null) return null;
    OnRayCast?.Invoke(node);
    return node;
  }
}