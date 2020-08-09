using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform m_transform;
    // 角色控制器组件
    CharacterController m_ch;
    //角色移动速度
    float m_moveSpeed = 3.0f;
    // 重力
    float m_gravity = 2.0f;
    // 生命值
    public int m_life = 5;
    // 摄像机Transform
    Transform m_camTransform;
    // 摄像机旋转角度
    Vector3 m_camRot;
    // 摄像机高度
    float m_camHeight = 1.8f;
    // 枪口
    Transform m_muzzlepoint;
    // 射击时，射线能射到的碰撞层
    public LayerMask m_layer;
    // 射中目标后的粒子效果
    public Transform m_fx;
    // 射击音效
    public AudioClip m_audio;
    // 设计间隔时间计时器
    float m_shootTimer = 0;
    GameObject M16;
    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        m_ch = this.GetComponent<CharacterController>();
        // 获取摄像机
        m_camTransform = Camera.main.transform;
        m_camTransform.position = m_transform.TransformPoint(0, m_camHeight, 0);
        m_camTransform.rotation = m_transform.rotation;
        m_camRot = m_camTransform.eulerAngles;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //获取枪口位置
        m_muzzlepoint = m_camTransform.Find("M16/weapon/muzzlepoint").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_life <= 0)
        {
            return;
        }
        Control();

    }
    void Control()
    {
        float rh = Input.GetAxis("Mouse X");
        float rv = Input.GetAxis("Mouse Y");
        m_camRot.x -= rv;
        m_camRot.y += rh;
        m_camTransform.eulerAngles = m_camRot;
        Vector3 camrot = m_camTransform.eulerAngles;
        camrot.x = 0;
        camrot.z = 0;
        m_transform.eulerAngles = camrot;

        Vector3 motion = Vector3.zero;
        motion.x = Input.GetAxis("Horizontal") * m_moveSpeed * Time.deltaTime;
        motion.z = Input.GetAxis("Vertical") * m_moveSpeed * Time.deltaTime;
        motion.y -= m_gravity * Time.deltaTime;
        m_ch.Move(m_transform.TransformDirection(motion));

        m_camTransform.position = m_transform.TransformPoint(0, m_camHeight, 0);

        m_shootTimer -= Time.deltaTime;
        if (Input.GetMouseButton(0) && m_shootTimer <= 0)
        {
            m_shootTimer = 0.1f;
            this.GetComponent<AudioSource>().PlayOneShot(m_audio);
            GameManager.Instance.SetAmmo(1);
            RaycastHit info;
            bool hit = Physics.Raycast(m_muzzlepoint.position, m_camTransform.TransformDirection(Vector3.forward), out info, 100, m_layer);
            if (hit)
            {
                if (info.transform.tag.CompareTo("enemy") == 0)
                {
                    Enemy_Nurse enemy = info.transform.GetComponent<Enemy_Nurse>();
                    //Enemy enemy = info.transform.GetComponent<Enemy>();
                    enemy.OnDamage(1);

                }
                Instantiate(m_fx, info.point, info.transform.rotation);
            }
        }
        m_ch.Move(m_transform.TransformDirection(motion));
    }
    public void OnDamage(int damage)
    {
        m_life -= damage;
        GameManager.Instance.SetLife(m_life);
        if (m_life <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
    }
}

