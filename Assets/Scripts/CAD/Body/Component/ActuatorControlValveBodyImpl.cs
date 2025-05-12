using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chiyoda.CAD.Model;

namespace Chiyoda.CAD.Body
{

    public class ActuatorControlValveBodyImpl : MonoBehaviour
    {
        [SerializeField]
        GameObject mainValve;

        [SerializeField]
        GameObject a;
        [SerializeField]
        GameObject d;
        [SerializeField]
        GameObject d_Hold;
        [SerializeField]
        GameObject A_cylinder;

        public GameObject A
        {
            get
            {
                return a;
            }

            set
            {
                a = value;
            }
        }
        public GameObject D
        {
            get
            {
                return d;
            }

            set
            {
                d = value;
            }
        }
        public GameObject MainValve
        {
            get
            {
                return mainValve;
            }

            set
            {
                mainValve = value;
            }
        }
        public GameObject D_Hold
        {
            get
            {
                return d_Hold;
            }

            set
            {
                d_Hold = value;
            }
        }
        public GameObject A_Cylinder
        {
            get
            {
                return A_cylinder;
            }

            set
            {
                A_cylinder = value;
            }
        }
    }
}
