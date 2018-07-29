using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMovements : EnemyMovements {	
	// Update is called once per frame
	void FixedUpdate () {
        LookForAgro();
	}
}
