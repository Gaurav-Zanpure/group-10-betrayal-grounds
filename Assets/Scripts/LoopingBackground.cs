using UnityEngine;

public class LoopingBackground : MonoBehaviour {
    public float length; // Width of the background
    public Transform player; // Reference to the player

    void Update() {
        if (player.position.x > transform.position.x + length) {
            transform.position += new Vector3(length * 2, 0, 0); // Move background forward
        }
    }
}
