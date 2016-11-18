using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class EventHandlers : MonoBehaviour
{
	public delegate void MinimalHandler();

	public delegate void BasicHandler(Object sender);

	public delegate void GameObjectHandler( GameObject sender );
	
	public delegate void BasicGameObjectHandler(Object sender);

	public delegate void Handler(Object sender, EventArgs args);

	public delegate void FloatHandler( float f );

	public delegate void VectorHandler( Vector3 v );

	public delegate void IntHandler( int i );

	public delegate void BoolHandler( bool b );

	public delegate void StringHandler( string s );

	public delegate void CollisionHandler( Collision other );
}