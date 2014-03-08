//PlayerStats is an exemplary script for giving and manipulating simple stats in your Player.

using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour 
{
	public enum PlayerState
	{
		PS_Alive, PS_Dead	
	}
	
	private PlayerState playerState;
	
	public int level = 1;
	public int experience = 0;
	
	public int strength = 1;
	public int hitPoints = 10;
	
	public int damageModifierOfWeapon = 0;
	
	void Awake()
	{
		playerState = PlayerState.PS_Alive;
	}
	
	public int GetPlayerDamage()
	{
		return (strength + damageModifierOfWeapon);	
	}
	
	public void GetHit(int damage)
	{
		hitPoints -= damage;
		if(hitPoints < 1 && playerState == PlayerState.PS_Alive) 
		{
			playerState = PlayerState.PS_Dead;
			//Stop the game.
			Debug.Log("You died.");
		}
	}
	
	

}
