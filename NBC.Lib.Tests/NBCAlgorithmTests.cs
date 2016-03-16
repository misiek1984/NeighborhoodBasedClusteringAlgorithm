using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NBC.Lib.Tests
{
    [DeploymentItem("sample1", "")]
    [DeploymentItem("sample2", "")]
    [TestClass]
    public class NBCAlgorithmTests
    {
        [TestMethod]
        public void NBCAlgorithm_Sample1_k3()
        {
            var reader = new DataReader();
            var vectors = reader.ReadData("sample1");
            var alg = new NBCAlgorithm(vectors);
            var results = alg.Start(3);

            var expected = new List<int>
            {
                0,
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1,
                1,
                2,
                2,
                2,
                2,
                2
            };

            Assert.AreEqual(results.Count, expected.Count);

            for (var i = 0; i < results.Count; ++i)
                Assert.AreEqual(results[i], expected[i]);

        }

        [TestMethod]
        public void NBCAlgorithm_Sample2_k9()
        {
            var reader = new DataReader();
            var vectors = reader.ReadData("sample2");
            var alg = new NBCAlgorithm(vectors);
            var results = alg.Start(9);

            var expected = new List<int>
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                -1,
                -1,
                -1,
                -1,
                -1,
                -1
            };

            Assert.AreEqual(results.Count, expected.Count);

            for (var i = 0; i < results.Count; ++i)
                Assert.AreEqual(results[i], expected[i]);

        }
    }
}
