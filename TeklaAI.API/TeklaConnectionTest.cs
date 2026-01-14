using System;
using System.CodeDom;
using System.Reflection;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace TeklaAI.API
{
    /// <summary>
    /// Simple test to verify Tekla API connection and basic operations
    /// </summary>
    public class TeklaConnectionTest
    {
        /// <summary>
        /// Test 1: Check if we can connect to Tekla Structures
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                Model model = new Model();
                bool isConnected = model.GetConnectionStatus();

                if (isConnected)
                {
                    Console.WriteLine("✓ SUCCESS: Connected to Tekla Structures");

                    // Get some model info
                    ModelInfo info = model.GetInfo();
                    Console.WriteLine($"  Model Name: {info.ModelName}");
                    Console.WriteLine($"  Model Path: {info.ModelPath}");

                    return true;
                }
                else
                {
                    Console.WriteLine("✗ FAILED: Tekla Structures is not running or no model is open");
                    Console.WriteLine("  → Please open Tekla Structures and open/create a model");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ERROR: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test 2: Create a simple beam in the model
        /// </summary>
        public static bool TestCreateBeam()
        {
            try
            {
                Model model = new Model();

                if (!model.GetConnectionStatus())
                {
                    Console.WriteLine("✗ Cannot create beam: Not connected to Tekla");
                    return false;
                }

                // Define beam start and end points (in millimeters)
                Point startPoint = new Point(0, 0, 0);
                Point endPoint = new Point(5000, 0, 0);  // 5 meter beam along X-axis

                // Create the beam
                Beam beam = new Beam(startPoint, endPoint);
                beam.Profile.ProfileString = "UB 305X165X40";  // UK Universal Beam
                beam.Material.MaterialString = "S355";         // Steel grade
                beam.Class = "1";                              // Class for drawings
                beam.Name = "TEST BEAM";
                beam.Finish = "PAINTED";

                // Insert the beam into the model
                bool success = beam.Insert();

                if (success)
                {
                    // Commit changes to make them visible in Tekla
                    model.CommitChanges();

                    Console.WriteLine("✓ SUCCESS: Created test beam");
                    Console.WriteLine($"  Profile: {beam.Profile.ProfileString}");
                    Console.WriteLine($"  Material: {beam.Material.MaterialString}");
                    Console.WriteLine($"  Length: 5000mm (5 meters)");
                    Console.WriteLine($"  → Check your Tekla model - you should see the beam!");

                    return true;
                }
                else
                {
                    Console.WriteLine("✗ FAILED: Could not insert beam into model");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ERROR creating beam: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test 3: Count existing beams in the model
        /// </summary>
        public static bool TestReadModel()
        {
            try
            {
                Model model = new Model();

                if (!model.GetConnectionStatus())
                {
                    Console.WriteLine("✗ Cannot read model: Not connected to Tekla");
                    return false;
                }

                // Count all beams in the model
                int beamCount = 0;
                ModelObjectEnumerator beamEnumerator = model.GetModelObjectSelector()
                    .GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

                while (beamEnumerator.MoveNext())
                {
                    beamCount++;
                }

                Console.WriteLine("✓ SUCCESS: Read model data");
                Console.WriteLine($"  Total beams in model: {beamCount}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ERROR reading model: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Run all tests in sequence
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("  TEKLA API CONNECTION TEST");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();

            Console.WriteLine("Test 1: Checking Tekla connection...");
            bool test1 = TestConnection();
            Console.WriteLine();

            if (test1)
            {
                Console.WriteLine("Test 2: Creating a test beam...");
                bool test2 = TestCreateBeam();
                Console.WriteLine();

                Console.WriteLine("Test 3: Reading model data...");
                bool test3 = TestReadModel();
                Console.WriteLine();

                Console.WriteLine("═══════════════════════════════════════════");
                if (test1 && test2 && test3)
                {
                    Console.WriteLine("  ✓ ALL TESTS PASSED!");
                    Console.WriteLine("  Your Tekla API setup is working correctly.");
                }
                else
                {
                    Console.WriteLine("  ⚠ SOME TESTS FAILED");
                    Console.WriteLine("  Check the errors above.");
                }
                Console.WriteLine("═══════════════════════════════════════════");
            }
            else
            {
                Console.WriteLine("═══════════════════════════════════════════");
                Console.WriteLine("  Cannot proceed with other tests.");
                Console.WriteLine("  Please start Tekla Structures first.");
                Console.WriteLine("═══════════════════════════════════════════");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}