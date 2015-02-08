﻿Namespace Util
    ''' <summary>
    ''' Xorshift random algorithm
    ''' Inherits System.Random
    ''' </summary>
    ''' <remarks>
    ''' Refference:
    ''' George Marsaglia, "Xorshift RNGs", Journal of Statistical Software Vol. 8, Issue 14, Jul 2003
    ''' </remarks>
    Public Class clsRandomXorshift : Inherits System.Random
        'DefaultParameter
        Private x As UInteger = 123456789
        Private y As UInteger = 362436069
        Private z As UInteger = 521288629
        Private w As UInteger = 88675123
        Private t As UInteger

#Region "Public"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' Constructor with seed
        ''' </summary>
        ''' <param name="ai_seed"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_seed As UInteger)
            Me.SetSeed(ai_seed)
        End Sub
#End Region

        ''' <summary>
        ''' Sample
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' The Sample method generates a distribution proportional to the value of the random numbers, in the range [0.0, 1.0].
        ''' </remarks>
        Protected Overrides Function Sample() As Double
            Return Me.Xor128() / UInteger.MaxValue
        End Function

        ''' <summary>
        ''' Set random seed
        ''' </summary>
        ''' <param name="ai_seed"></param>
        ''' <remarks></remarks>
        Public Sub SetSeed(Optional ByVal ai_seed As UInteger = 88675123)
            '"The seed set for xor128 is four 32-bit integers x,y,z,w not all 0" by refference

            If ai_seed = 88675123 Then
                'using default parameter
                x = 123456789
                y = 362436069
                z = 521288629
                w = 88675123
                t = 0
            Else
                'Init parameter
                '全パラメータにseedの影響を与えないと初期の乱数が同じ傾向になる。8bitずつ回転左シフト
                x = x Xor Me.RotateLeftShiftForUInteger(ai_seed, 8)
                y = y Xor Me.RotateLeftShiftForUInteger(ai_seed, 16)
                z = z Xor Me.RotateLeftShiftForUInteger(ai_seed, 24)
                w = w Xor ai_seed 'Set seed
                t = 0
            End If
        End Sub

        ''' <summary>
        ''' Rotate Shift
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RotateLeftShiftForUInteger(ByVal ai_val As UInteger, ByVal ai_leftshit As Integer) As UInteger
            If 32 - ai_leftshit <= 0 Then
                Return ai_val
            End If
            Dim upper As UInteger = CUInt(ai_val And &HFF000000)
            upper = upper >> (ai_leftshit - 32)
            Dim temp As UInteger = ai_val << ai_leftshit
            ai_val = temp Or upper
            Return ai_val
        End Function

        ''' <summary>
        ''' Random double with range
        ''' </summary>
        ''' <param name="ai_min"></param>
        ''' <param name="ai_max"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Overloads Function NextDouble(ByVal ai_min As Double, ByVal ai_max As Double) As Double
            Dim ret As Double = Math.Abs(ai_max - ai_min) * MyBase.NextDouble() + ai_min
            Return ret
        End Function

        ''' <summary>
        ''' for random seed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTimeSeed() As UInteger
            Return CUInt(Date.Now.Millisecond * Date.Now.Minute * Date.Now.Second)
        End Function

#Region "Private"
        ''' <summary>
        ''' Xor128
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' C Source by refference
        '''  t=(xˆ(x leftshift 11));
        '''  x=y;
        '''  y=z;
        '''  z=w;
        '''  return( w=(wˆ(w rightshift 19))ˆ(tˆ(t rightshift 8)) )
        ''' </remarks>
        Private Function Xor128() As UInteger
            t = (x Xor (x << 11))
            x = y
            y = z
            z = w
            w = (w Xor (w >> 19)) Xor (t Xor (t >> 8))

            Return w
        End Function
#End Region
    End Class

    ''' <summary>
    ''' Xorshift random algorithm singleton
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class clsRandomXorshiftSingleton
        Private Shared m_rand As New clsRandomXorshift()

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub New()
            'nop
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Instance
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetInstance() As clsRandomXorshift
            Return m_rand
        End Function

        ''' <summary>
        ''' Set Seed
        ''' </summary>
        ''' <param name="ai_seed"></param>
        ''' <remarks></remarks>
        Public Sub SetSeed(Optional ByVal ai_seed As UInteger = 123456)
            clsRandomXorshiftSingleton.GetInstance().SetSeed(ai_seed)
        End Sub
#End Region
    End Class
End Namespace
