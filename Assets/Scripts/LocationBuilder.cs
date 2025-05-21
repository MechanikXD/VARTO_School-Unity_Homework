using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationBuilder : MonoBehaviour {
    void Start() => GenerateGround();

    private void GenerateGround() {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.transform.localScale = new Vector3(50, 1, 1);
        var groundRenderer = ground.GetComponent<Renderer>();
        groundRenderer.material.color = Color.black;
        SceneManager.MoveGameObjectToScene(ground, SceneManager.GetActiveScene());
    }
}