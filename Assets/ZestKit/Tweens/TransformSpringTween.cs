﻿using UnityEngine;
using System.Collections;


namespace Prime31.ZestKit
{
	public class TransformSpringTween : AbstractTweenable
	{
		Transform _transform;
		TransformTargetType _targetType;
		Vector3 _targetValue;
		Vector3 _velocity;

		// these are public for easier value tweaking at design time
		public float dampingRatio = 0.23f;
		public float angularFrequency = 25;


		/// <summary>
		/// Initializes a new instance of the <see cref="Prime31.ZestKit.TransformSpringTween"/> class.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <param name="dampingRatio">lower values are less damped and higher values are more damped resulting in less springiness.
		/// should be between 0.01f, 1f to avoid unstable systems.</param>
		/// <param name="angularFrequency">An angular frequency of 2pi (radians per second) means the oscillation completes one
		/// full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable</param>
		public TransformSpringTween( Transform transform, TransformTargetType taretType, Vector3 targetValue )
		{
			_transform = transform;
			_targetType = taretType;
			setTargetValue( targetValue );
		}


		/// <summary>
		/// you can call setTargetValue at any time to reset the target value to a new Vector3. If you have not called start to add the
		/// spring tween to ZestKit it will be called for you.
		/// </summary>
		/// <param name="targetValue">Target value.</param>
		public void setTargetValue( Vector3 targetValue )
		{
			_velocity = Vector3.zero;
			_targetValue = targetValue;

			if( !_isActiveTween )
				start();
		}


		/// <summary>
		/// lambda should be the desired duration when the oscillation magnitude is reduced by 50%
		/// </summary>
		/// <param name="lambda">Lambda.</param>
		public void updateDampingRatioWithHalfLife( float lambda )
		{
			dampingRatio = ( -lambda / angularFrequency ) * Mathf.Log( 0.5f );
		}


		#region AbstractTweenable

		public override bool tick()
		{
			if( _isPaused )
				return false;

			setTweenedValue( Zest.fastSpring( getCurrentValueOfTweenedTargetType(), _targetValue, ref _velocity, dampingRatio, angularFrequency ) );

			return false;
		}

		#endregion


		void setTweenedValue( Vector3 value )
		{
			// if the babysitter is enabled and we dont validate just silently do nothing
			if( ZestKit.enableBabysitter && !_transform )
				return;

			switch( _targetType )
			{
				case TransformTargetType.Position:
					_transform.position = value;
					break;
				case TransformTargetType.LocalPosition:
					_transform.localPosition = value;
					break;
				case TransformTargetType.LocalScale:
					_transform.localScale = value;
					break;
				case TransformTargetType.EulerAngles:
					_transform.eulerAngles = value;
					break;
				case TransformTargetType.LocalEulerAngles:
					_transform.localEulerAngles = value;
					break;
				default:
					throw new System.ArgumentOutOfRangeException();
			}
		}


		Vector3 getCurrentValueOfTweenedTargetType()
		{
			switch( _targetType )
			{
				case TransformTargetType.Position:
					return _transform.position;
				case TransformTargetType.LocalPosition:
					return _transform.localPosition;
				case TransformTargetType.LocalScale:
					return _transform.localScale;
				case TransformTargetType.EulerAngles:
					return _transform.eulerAngles;
				case TransformTargetType.LocalEulerAngles:
					return _transform.localEulerAngles;
			}

			return Vector3.zero;
		}

	}
}