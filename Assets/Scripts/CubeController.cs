using CustomMath;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CubeController;

public class CubeController : MonoBehaviour
{
    [SerializeField] private Transform t00;
    [SerializeField] private Transform t01;
    [SerializeField] private Transform t02;
    [SerializeField] private Transform t10;
    [SerializeField] private Transform t11;//
    [SerializeField] private Transform t12;
    [SerializeField] private Transform t20;
    [SerializeField] private Transform t21;
    [SerializeField] private Transform t22;

    [SerializeField] private Transform m00;
    [SerializeField] private Transform m01;//
    [SerializeField] private Transform m02;
    [SerializeField] private Transform m10;//
    [SerializeField] private Transform m11;//
    [SerializeField] private Transform m12;//
    [SerializeField] private Transform m20;
    [SerializeField] private Transform m21;//
    [SerializeField] private Transform m22;

    [SerializeField] private Transform b00;
    [SerializeField] private Transform b01;
    [SerializeField] private Transform b02;
    [SerializeField] private Transform b10;
    [SerializeField] private Transform b11;//
    [SerializeField] private Transform b12;
    [SerializeField] private Transform b20;
    [SerializeField] private Transform b21;
    [SerializeField] private Transform b22;

    private List<Part> parts = new List<Part>();
    private Transform[] rotators;

    private Part t00p;
    private Part t01p;
    private Part t02p;
    private Part t10p;
    private Part t12p;
    private Part t20p;
    private Part t21p;
    private Part t22p;

    private Part m00p;
    private Part m01p;
    private Part m02p;
    private Part m10p;
    private Part m12p;
    private Part m20p;
    private Part m21p;
    private Part m22p;

    private Part b00p;
    private Part b01p;
    private Part b02p;
    private Part b10p;
    private Part b12p;
    private Part b20p;
    private Part b21p;
    private Part b22p;

    private bool move = false;
    private float angle = 0f;
    private const float fixedRotation = 90f;

    [SerializeField] private float distToParent;

    private Transform rotator;

    public class Part
    {
        public Transform trs;
        public Transform father;
        public Mat4x4 centerOffset;
        float closest = 0f;
        public bool isActive = false;

        public Part()
        {

        }

        public Part(Transform trs, Transform father)
        {
            this.trs = trs;
            this.father = father;
        }
        public void OnUpdate()
        {
            Mat4x4 axisCenter = Mat4x4.TRS(father.position, father.rotation, father.lossyScale);

            Mat4x4 newTrs = axisCenter * centerOffset;

            trs.position = newTrs.GetPosition();
            trs.rotation = newTrs.rotation;
        }
        public Mat4x4 CalculateOffset()
        {
            return centerOffset = Mat4x4.Inverse(Mat4x4.TRS(father.position, father.rotation, father.lossyScale))
                        * Mat4x4.TRS(trs.position, trs.rotation, trs.lossyScale);
        }
    }
    [SerializeField] private Transform cube;

    private void OnEnable()
    {
        rotator = t11;
    }
    private void Start()
    {

        t00p = new Part(t00, t11);
        t01p = new Part(t01, t11);
        t02p = new Part(t02, t11);
        t10p = new Part(t10, t11);
        t12p = new Part(t12, t11);
        t20p = new Part(t20, t11);
        t21p = new Part(t21, t11);
        t22p = new Part(t22, t11);

        m00p = new Part(m00, m11);
        m02p = new Part(m02, m11);
        m20p = new Part(m20, m11);
        m22p = new Part(m22, m11);

        b00p = new Part(b00, m11);
        b01p = new Part(b01, m11);
        b02p = new Part(b02, m11);
        b10p = new Part(b10, m11);
        b12p = new Part(b12, m11);
        b20p = new Part(b20, m11);
        b21p = new Part(b21, m11);
        b22p = new Part(b22, m11);

        parts.Add(t00p);
        parts.Add(t01p);
        parts.Add(t02p);
        parts.Add(t10p);
        parts.Add(t12p);
        parts.Add(t20p);
        parts.Add(t21p);
        parts.Add(t22p);
        parts.Add(m00p);
        parts.Add(m02p);
        parts.Add(m20p);
        parts.Add(m22p);
        parts.Add(b00p);
        parts.Add(b01p);
        parts.Add(b02p);
        parts.Add(b10p);
        parts.Add(b12p);
        parts.Add(b20p);
        parts.Add(b21p);
        parts.Add(b22p);
    }

    private void Update()
    {
        move = false;

        if (Input.GetKeyDown(KeyCode.W))
        {
            rotator = t11;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            rotator = m01;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            rotator = m10;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rotator = m12;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotator = m21;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotator = b11;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            move = true;
            angle = -1;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            move = true;
            angle = 1;
        }


        if (move)
        {
            foreach (Part part in parts)
            {
                part.isActive = false;
                Debug.Log(Vec3.Distance(rotator.position, part.trs.position));

                if (Vec3.Distance(rotator.position, part.trs.position) < distToParent)
                {
                    part.father = rotator;
                    part.isActive = true;
                    part.CalculateOffset();
                }
            }
            Vec3 rotationAxis = (rotator.position - m11.position).normalized;
            Quat newRot = Quat.AngleAxis(fixedRotation * angle, rotationAxis);
            Quat currentRot = rotator.rotation;
            newRot *= currentRot;

            rotator.rotation = newRot;

            foreach (Part part in parts)
            {
                if (part.isActive)
                {
                    part.OnUpdate();
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (rotator != null)
        {
            Gizmos.DrawSphere(rotator.transform.position + (rotator.transform.position - m11.position).normalized * 0.1f, 0.05f);
        }
    }
}
