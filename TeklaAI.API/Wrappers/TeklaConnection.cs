using System;
using Tekla.Structures.Model;
using TeklaAI.Core.Interfaces;
using TeklaAI.Core.Utilities;

namespace TeklaAI.API.Wrappers
{
    /// <summary>
    /// Wrapper for Tekla Structures connection and basic operations
    /// </summary>
    public class TeklaConnection
    {
        private readonly ILogger _logger;
        private Model _model;

        public TeklaConnection()
        {
            _logger = Logger.Instance;
        }

        /// <summary>
        /// Gets the current Tekla Model instance
        /// </summary>
        public Model Model => _model;

        /// <summary>
        /// Check if connected to Tekla Structures
        /// </summary>
        public bool IsConnected => _model != null && _model.GetConnectionStatus();

        /// <summary>
        /// Connect to Tekla Structures
        /// </summary>
        /// <returns>True if connected successfully</returns>
        public bool Connect()
        {
            try
            {
                _logger.Info("Attempting to connect to Tekla Structures...");

                _model = new Model();

                if (!_model.GetConnectionStatus())
                {
                    _logger.Error("Tekla Structures is not running or no model is open.");
                    return false;
                }

                // Get model information
                ModelInfo modelInfo = _model.GetInfo();
                _logger.Success($"Connected to Tekla Structures successfully!");
                _logger.Info($"Model Name: {modelInfo.ModelName}");
                _logger.Info($"Model Path: {modelInfo.ModelPath}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to connect to Tekla Structures", ex);
                return false;
            }
        }

        /// <summary>
        /// Commit changes to the model
        /// </summary>
        public bool CommitChanges()
        {
            try
            {
                if (!IsConnected)
                {
                    _logger.Error("Cannot commit changes: Not connected to Tekla");
                    return false;
                }

                _logger.Info("Committing changes to model...");
                _model.CommitChanges();
                _logger.Success("Changes committed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to commit changes", ex);
                return false;
            }
        }

        /// <summary>
        /// Get model information
        /// </summary>
        public ModelInfo GetModelInfo()
        {
            if (!IsConnected)
            {
                _logger.Warning("Not connected to Tekla. Cannot retrieve model info.");
                return null;
            }

            try
            {
                return _model.GetInfo();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get model information", ex);
                return null;
            }
        }

        /// <summary>
        /// Test method: Create a simple beam - tries multiple profile formats
        /// </summary>
        public bool CreateTestBeam()
        {
            try
            {
                if (!IsConnected)
                {
                    _logger.Error("Cannot create beam: Not connected to Tekla");
                    return false;
                }

                _logger.Info("Creating test beam...");

                // Create a beam from (0,0,0) to (5000,0,0) - 5 meters long
                Tekla.Structures.Geometry3d.Point startPoint = new Tekla.Structures.Geometry3d.Point(0, 0, 0);
                Tekla.Structures.Geometry3d.Point endPoint = new Tekla.Structures.Geometry3d.Point(5000, 0, 0);

                // Try different profile formats that are commonly available
                string[] profilesToTry = new string[]
                {
                    "HEA200",           // European standard
                    "IPE200",           // European I-beam
                    "UB305*165*40",     // UK Universal Beam
                    "W10X49",           // US Wide Flange
                    "RHS200*100*8",     // Rectangular Hollow Section
                    "UC254*254*89"      // UK Universal Column
                };

                foreach (string profileString in profilesToTry)
                {
                    _logger.Info($"Trying profile: {profileString}");

                    Beam beam = new Beam
                    {
                        StartPoint = startPoint,
                        EndPoint = endPoint
                    };

                    beam.Profile.ProfileString = profileString;
                    beam.Material.MaterialString = "S355";
                    beam.Class = "1";
                    beam.Finish = "PAINT";

                    bool inserted = beam.Insert();

                    if (inserted)
                    {
                        _logger.Success($"Test beam created successfully with profile {profileString}!");
                        _logger.Info($"Beam ID: {beam.Identifier.ID}");
                        CommitChanges();
                        return true;
                    }
                    else
                    {
                        _logger.Warning($"Profile {profileString} failed, trying next...");
                    }
                }

                _logger.Error("Failed to insert beam with any available profile");
                _logger.Warning("Please check your Tekla profile catalog or add profiles manually");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception while creating test beam", ex);
                return false;
            }
        }

        /// <summary>
        /// Get count of all beams in the model
        /// </summary>
        public int GetBeamCount()
        {
            try
            {
                if (!IsConnected)
                {
                    _logger.Warning("Not connected to Tekla");
                    return -1;
                }

                int count = 0;
                ModelObjectEnumerator enumerator = _model.GetModelObjectSelector()
                    .GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

                while (enumerator.MoveNext())
                {
                    count++;
                }

                _logger.Info($"Model contains {count} beams");
                return count;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to count beams", ex);
                return -1;
            }
        }
    }
}