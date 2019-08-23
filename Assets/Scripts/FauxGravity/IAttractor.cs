using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FauxGravity {
    public interface IAttractor {
        Vector3 Attract(Body body);
        GameObject gameObject { get; }

        SphereCollider attractorRange { get; }
    }
}
