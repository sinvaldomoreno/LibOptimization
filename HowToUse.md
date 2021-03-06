# How to use LibOptimization

This tutrial, You design a objective function to find the minimum value of the [2D Sphere function](https://qiita.com/tomitomi3/items/d4318bf7afbc1c835dda#sphere-function). 
This function is a unimodal convex function and has a global minimum value.

**Optimization flow using LibOptimization**

1. Get LibOptimization your solution from Nuget.
1. You inherit "absObjectiveFunction" class and design the objective function.
1. Choose an optimization method and implement code.
1. Do optimization.
1. Get result and evaluate.

## preparation

Create a console application and development language is C#.
In this example, You use C#. You can also use VisualBasic.NET.

## Step1. Get LibOptimization

URL:https://www.nuget.org/packages/LibOptimization/
```
PM> Install-Package LibOptimization
```

## Step2. Design objective fucntion

Add an objective function class that inherit **absObjectiveFunction** to the solution.

absObjectiveFunction is the base class for objective functions in the LibOptimization.

**SphereFunction.cs**
```csharp
    /// <summary>
    /// objective function inherit absObjectiveFunction
    /// </summary>
    class SphereFunction : LibOptimization.Optimization.absObjectiveFunction
    {
        public SphereFunction()
        {
        }

        /// <summary>
        /// design objective function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double F(List<double> x)
        {
            var ret = 0.0; 
            var dim = this.NumberOfVariable(); //or x.Count
            for (int i = 0; i < dim; i++)
            {
                ret += x[i] * x[i];// x^2
            }
            return ret;
        }

        /// <summary>
        /// Gradient of the objective function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override List<double> Gradient(List<double> x)
        {
            //If you use the gradient method or Newton method, implement the derivative of the objective function. otherwise, return null.
            return null;
        }

        /// <summary>
        /// Hessian matrix of the objective function
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        public override List<List<double>> Hessian(List<double> x)
        {
            //If you use the Newton method, implement the derivative of the objective function. otherwise, return null.
            return null;
        }

        /// <summary>
        /// The number of dimensions of the objective function
        /// </summary>
        /// <returns></returns>
        public override int NumberOfVariable()
        {
            return 2;
        }
    }
```

**Gradient(List<double> x)**, **Hessian(List<double> x)** implement the derivative of the objective function. However, it is a little difficult to implement it.

```csharp
        public override List<double> Gradient(List<double> x)
        {
            //Differentiation of sphere function
            // f(x) = x^2
            // df/dx = 2 * x
            var ret = new List<double>();
            var dim = this.NumberOfVariable(); //or x.Count
            for (int i = 0; i < dim; i++)
            {
                ret.Add(2.0 * x[i]);
            }
            return ret;
        }
        
        public override List<List<double>> Hessian(List<double> x)
        {
            // Hessian of sphere function
            // H =
            // | d^2 f/d^2x1 df1/dx2     |
            // | df2/dx1     d^2 f/d^2x2 |
            var h = new List<List<double>>();
            h.add( new List<double>());
            h[0].add(0.0);
            h[0].add(2.0);
            h.add( new List<double>());
            h[1].add(2.0);
            h[1].add(0.0);
            return h;
        }
```

If the function is smooth, it can be approximated by using numerical differentiation.
Numerical differentiation of the objective function can be easily implemented using the following API.

Hessian uses Newton method only. Newton method can be optimized correctly only if the Hessian matrix is positive definite and the initial values are not near the solution.

```csharp
        public override List<double> Gradient(List<double> x)
        {
            return base.NumericDerivative(x);
        }
        
        public override List<List<double>> Hessian(List<double> x)
        {
            //NumericHessianToDiagonal() stores the second derivative of the objective function in the diagonal component.
            return base.NumericHessianToDiagonal(x);
        }
```

## Step3. Choose an optimization method

Write the code to set the evaluation function into the optimization algorithm. Typical code is as follows.

Choosing an optimization algorithm requires experience. In this example, using PSO (Particle Swarm Optimization).

**Program.cs**
```csharp
    class Program
    {
        static void Main(string[] args)
        {
            var func = new SphereFunction();

            //Set objective function to optimizeclass
            var opt = new LibOptimization.Optimization.clsOptPSO(func);

            //Initialize(generate initial value)
            opt.Init();

            //Do Optimization
            opt.DoIteration();

            //Get result
            var result = opt.Result;

            //output console
            Console.WriteLine("Eval : {0}", result.Eval);
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine("{0}", result[i]);
            }
        }
    }
```

## Step4. Buld and Run

Build and run the program. You should see the results in the console after a while.

## Step5. Review and evaluation of results

Check the results obtained.

Are the results extremely large?

Not getting enough iteration?

etc.

# Tips

* Not using stopping criteria

The implemented optimization algorithm has a stopping criterion. This stopping criterion is stopped when the best evaluate value is equal to 70% of the population.

**IsUseCriterion** property is **false**.

```csharp
//Set objective function to optimizeclass
var opt = new LibOptimization.Optimization.clsOptPSO(func);

opt.IsUseCriterion = false; //not use criteria

//Initialize(generate initial value)
opt.Init();
```

* Evaluate optimization result per 100 iteration

```csharp
//Evaluate optimization result per 100 iteration
while (opt.DoIteration(100) == false)
{
    clsUtil.DebugValue(opt, ai_isOutValue: false);
}
clsUtil.DebugValue(opt);
```

* Reset Iteration count

```csharp
    class Program
    {
        static void Main(string[] args)
        {
            var func = new SphereFunction();

            //Set objective function to optimizeclass
            var opt = new LibOptimization.Optimization.clsOptPSO(func);

            //Initialize(generate initial value)
            opt.Init();

            //Do Optimization
            opt.DoIteration();

            //Get result
            var result = opt.Result;

            //output console
            Console.WriteLine("Eval : {0}", result.Eval);
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine("{0}", result[i]);
            }

            //Reset iteration
            opt.Iteration = 0;
            opt.DoIteration();

            //Get result
            var result = opt.Result;

            //output console
            Console.WriteLine("Eval : {0}", result.Eval);
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine("{0}", result[i]);
            }
        }
    }
```

* Saving and Restoring Optimization Calculations

You can export and restore the optimization results in binary format. **BinaryFormatter** is used to achieve this functionality.

**Save(Serialize)**
```csharp
LibOptimization.Util.clsUtil.SerializeOpt((absOptimization)opt, "saveOptimization.bin");
```

**Restore(DeSerialize)**
```csharp
var restoreOpt = LibOptimization.Util.clsUtil.DeSerializeOpt("saveOptimization.bin");
```

* fix Random Number Generator

Fix the seed of the random number generator. It should be used when you want reproducibility.

```csharp
//This RNG is used for random sequence etc.
LibOptimization.Util.clsRandomXorshiftSingleton.GetInstance().SetDefaultSeed();

var func = new RosenBrock(2);
var opt = new LibOptimization.Optimization.clsOptPSO(func);
//This RNG is used for generate itinial value, position etc.
opt.Random = new LibOptimization.Util.clsRandomXorshift();

//init
opt.Init();
```
