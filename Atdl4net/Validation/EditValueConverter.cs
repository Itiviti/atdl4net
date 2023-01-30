#region Copyright (c) 2010-2012, Cornerstone Technology Limited. http://atdl4net.org
//
//   This software is released under both commercial and open-source licenses.
//
//   If you received this software under the commercial license, the terms of that license can be found in the
//   Commercial.txt file in the Licenses folder.  If you received this software under the open-source license,
//   the following applies:
//
//      This file is part of Atdl4net.
//
//      Atdl4net is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public
//      License as published by the Free Software Foundation, either version 2.1 of the License, or (at your option) any later version.
//
//      Atdl4net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//      of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.
//
//      You should have received a copy of the GNU Lesser General Public License along with Atdl4net.  If not, see
//      http://www.gnu.org/licenses/.
//
#endregion

using System;
using System.Globalization;
using System.Linq;
using Atdl4net.Diagnostics;
using Atdl4net.Diagnostics.Exceptions;
using Atdl4net.Fix;
using Atdl4net.Model.Reference;
using Atdl4net.Model.Types.Support;
using Atdl4net.Resources;
using Atdl4net.Utility;

namespace Atdl4net.Validation
{
    /// <summary>
    /// Provides value conversion for <see cref="Edit_t"/> evaluation.
    /// </summary>
    public static class EditValueConverter
    {
        private static readonly string ExceptionContext = typeof(EditValueConverter).FullName;

        /// <summary>
        /// Attempts to convert the second parameter value to a comparable type of the first parameter.
        /// </summary>
        /// <param name="typeInstanceToMatch">Instance of a the target comparable type.</param>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value as an <see cref="IComparable"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown if the value cannot be converted to the target type.</exception>
        /// <exception cref="FormatException">Thrown if the value cannot be converted into a valid numeric type.</exception>
        public static IComparable ConvertToComparableType(object typeInstanceToMatch, string value)
        {
            // If we don't have a valid type to convert to, then best leave the value alone.
            if (typeInstanceToMatch == null)
                return value;

            string type = typeInstanceToMatch.GetType().FullName;

            return type switch
            {
                "System.Decimal" => (IComparable)Convert.ToDecimal(value),
                "System.Boolean" => ConvertToBool(value),
                "System.Int32" => Convert.ToInt32(value),
                "System.UInt32" => Convert.ToUInt32(value),
                "System.Char" => Convert.ToChar(value),
                "System.DateTime" => FixDateTime.Parse(value, CultureInfo.InvariantCulture),
                "System.String" => value,
                "Atdl4net.Model.Reference.IsoCountryCode" => value.ParseAsEnum<IsoCountryCode>(),
                "Atdl4net.Model.Reference.IsoCurrencyCode" => value.ParseAsEnum<IsoCurrencyCode>(),
                "Atdl4net.Model.Reference.IsoLanguageCode" => value.ParseAsEnum<IsoLanguageCode>(),
                "Atdl4net.Model.Types.Support.MonthYear" => MonthYear.Parse(value),
                "Atdl4net.Model.Types.Support.Tenor" => Tenor.Parse(value),
                "Atdl4net.Model.Controls.Support.EnumState" => value,
                _ => throw ThrowHelper.New<InvalidCastException>(ExceptionContext,
                    ErrorMessages.DataConversionError1, value, type)
            };
        }

        private static bool ConvertToBool(string value)
        {
            if (value == null)
                throw ThrowHelper.New<InvalidFieldValueException>(ExceptionContext, ErrorMessages.IllegalUseOfNullError);

            switch (value.ToUpper())
            {
                case "N":
                    return false;

                case "Y":
                    return true;

                default:
                    return Boolean.Parse(value);
            }
        }
    }
}