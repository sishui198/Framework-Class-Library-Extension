﻿using System.Collections.Generic;
using Whathecode.System.Windows.Input.InputController.Trigger;


namespace Whathecode.System.Windows.Input.InputController
{
	/// <summary>
	///   A class which manages InputTrigger's.
	///   Add wanted InputTrigger's to this class and call Update() as much as possible.
	///   The InputTrigger's will be triggered when needed.
	/// </summary>
	/// <author>Steven Jeuris</author>
	public class InputController
	{
		readonly List<EventTrigger> _toRemove = new List<EventTrigger>();

		/// <summary>
		///   List of InputTrigger's which the InputController needs to manage.
		/// </summary>
		protected List<EventTrigger> Triggers { get; set; }

		/// <summary>
		///   Enable/disable input triggers.
		/// </summary>
		public bool Enabled { get; set; }


		public InputController()
		{
			Enabled = true;
			Triggers = new List<EventTrigger>();
		}


		public void AddTrigger( EventTrigger trigger )
		{
			Triggers.Add( trigger );
		}

		public void RemoveTrigger( EventTrigger trigger )
		{
			_toRemove.Add( trigger );
		}

		public void Update()
		{
			if ( Enabled )
			{
				foreach ( var trigger in _toRemove )
				{
					Triggers.Remove( trigger );
				}
				_toRemove.Clear();

				// TODO: Why is there sometimes a InvalidOperationException here? (Triggers collection modified.)
				foreach ( var trigger in Triggers )
				{
					trigger.Update();
				}
			}
		}
	}
}