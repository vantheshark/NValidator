using NUnit.Framework;

namespace NValidator.Test
{
    [TestFixture]
    public abstract class ResetDefaultValidatorFactoryTests
    {
        private IValidatorFactory _validatorFactory;

        [SetUp]
        public void RefreshFactory()
        {
            _validatorFactory = ValidatorFactory.Current;
            ValidatorFactory.Current = new DefaultValidatorFactory();
        }

        [TearDown]
        public void RestoreFactory()
        {
            ValidatorFactory.Current = _validatorFactory;
        }
    }
}
