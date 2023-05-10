using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UES;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
    }

    [ConsoleCommand("set-speed")]
    private void SetSpeed(float _spd)
    {
        speed = _spd;
    }

    [ConsoleCommand("set-name")]
    private void SetName(string name)
    {

    }

    [ConsoleCommand("set-lives")]
    private void SetLives(int lives)
    {

    }

    [ConsoleCommand("set-li")]
    private void SetLi(Vector2 pos)
    {
        Debug.Log(pos);
        transform.position = pos;

    }

    [ConsoleCommand("set-enemy")]
    private void SetEnemy(double enemyId, object enemy)
    {

    }
    [ConsoleCommand("get-alive")]
    private void IsAlive(bool check)
    {

    }

    [ConsoleCommand("move-left", "move")]
    public void MoveLeft()
    {
        Vector2 pos = transform.position;
        pos.x -= Time.deltaTime * speed;
        transform.position = pos;
    }

    [ConsoleCommand("move-right")]
    public void MoveRight()
    {
        Vector2 pos = transform.position;
        pos.x += Time.deltaTime * speed;
        transform.position = pos;

    }
}

class Helly
{
    [ConsoleCommand("hi")]
    public void Hi()
    {

    }
}