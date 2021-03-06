﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Reflection;
using Whathecode.System.ComponentModel.NotifyPropertyFactory;
using Whathecode.System.ComponentModel.NotifyPropertyFactory.Attributes;
using Whathecode.System.Reflection.Extensions;
using Whathecode.System.Windows.Input.CommandFactory;


namespace Whathecode.System.Windows.Aspects.ViewModel
{
	/// <summary>
	///   Aspect which when applied to a class, allows using the <see cref = "NotifyPropertyFactory{TEnum}" />
	///   and the <see cref = "CommandFactory{T}" /> without having to add it to the class,
	///   or delegate calls to it in the property getters and setters.
	/// </summary>
	/// <typeparam name = "TProperties">Enum type specifying all the notify properties.</typeparam>
	/// <typeparam name = "TCommands">Enum type specifying all the commands.</typeparam>
	[Serializable]
	[IntroduceInterface( typeof( INotifyPropertyChanged ), OverrideAction = InterfaceOverrideAction.Ignore )]
	public class ViewModelAspect<TProperties, TCommands> : IInstanceScopedAspect, IAspectProvider, INotifyPropertyChanged
	{
		[NonSerialized]
		object _instance;

		[NonSerialized]
		CommandFactory<TCommands> _commandFactory;

		[IntroduceMember( Visibility = Visibility.Private )]
		public CommandFactory<TCommands> CommandFactory
		{
			get { return _commandFactory; }
			private set { _commandFactory = value; }
		}

		[NonSerialized]
		NotifyPropertyFactory<TProperties> _propertyFactory;

		[IntroduceMember( Visibility = Visibility.Private )]
		public NotifyPropertyFactory<TProperties> PropertyFactory
		{
			get { return _propertyFactory; }
			private set { _propertyFactory = value; }
		}

		readonly List<NotifyPropertyAspect<TProperties>> _notifyPropertyAspects = new List<NotifyPropertyAspect<TProperties>>();

		[IntroduceMember( OverrideAction = MemberOverrideAction.Ignore )]
		public event PropertyChangedEventHandler PropertyChanged;


		public object CreateInstance( AdviceArgs adviceArgs )
		{
			var newAspect = (ViewModelAspect<TProperties, TCommands>)MemberwiseClone();
			newAspect._instance = adviceArgs.Instance;

			return newAspect;
		}

		public void RuntimeInitializeInstance()
		{
			CommandFactory = new CommandFactory<TCommands>( _instance );
			PropertyFactory = new NotifyPropertyFactory<TProperties>( _instance, () => PropertyChanged );

			_notifyPropertyAspects.ForEach( p => p.SetPropertyFactory( _instance, PropertyFactory ) );
		}

		public IEnumerable<AspectInstance> ProvideAspects( object targetElement )
		{
			var targetType = (Type)targetElement;

			Dictionary<MemberInfo, NotifyPropertyAttribute[]> attributedProperties
				= targetType.GetAttributedMembers<NotifyPropertyAttribute>( MemberTypes.Property );

			foreach ( var member in attributedProperties )
			{
				var attribute = member.Value[ 0 ];
				var propertyAspect = new NotifyPropertyAspect<TProperties>( (TProperties)attribute.GetId() );
				_notifyPropertyAspects.Add( propertyAspect );

				yield return new AspectInstance( member.Key, propertyAspect );
			}
		}
	}
}