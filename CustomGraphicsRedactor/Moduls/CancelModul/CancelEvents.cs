namespace CustomGraphicsRedactor.Moduls
{
    /// <summary>
    /// Модель описания информации о действии для отмены
    /// </summary>
    class CancelEvents
    {
        private object _cancelObject;
        private ECancelTypes _cancelType;

        /// <param name="cancelType">Тип действия</param>
        /// <param name="cancelObject">Объект и условия для отмены</param>
        public CancelEvents(ECancelTypes cancelType, object cancelObject)
        {
            _cancelType = cancelType;
            _cancelObject = cancelObject;
        }

        /// <summary>
        /// Возвращает объект и условия для отмены действия
        /// </summary>
        public object CancelObject => _cancelObject;

        /// <summary>
        /// Возвращает тип действия
        /// </summary>
        public ECancelTypes CancelType => _cancelType;
    }
}
