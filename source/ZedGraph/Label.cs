//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2006  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Drawing;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  /// <summary>
  /// Class that handles the data associated with text title and its associated font
  /// properties
  /// </summary>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.2 $ $Date: 2007-03-11 02:08:16 $ </version>
  [Serializable]
  public class Label : ICloneable, ISerializable
  {
    #region Constructors

    /// <summary>
    /// Constructor to build an <see cref="AxisLabel" /> from the text and the
    /// associated font properties.
    /// </summary>
    /// <param name="text">The <see cref="string" /> representing the text to be
    /// displayed</param>
    /// <param name="fontFamily">The <see cref="String" /> font family name</param>
    /// <param name="fontSize">The size of the font in points and scaled according
    /// to the <see cref="PaneBase.CalcScaleFactor" /> logic.</param>
    /// <param name="color">The <see cref="Color" /> instance representing the color
    /// of the font</param>
    /// <param name="isBold">true for a bold font face</param>
    /// <param name="isItalic">true for an italic font face</param>
    /// <param name="isUnderline">true for an underline font face</param>
    public Label(string text, string fontFamily, float fontSize, Color color, bool isBold,
      bool isItalic, bool isUnderline)
    {
      _text = text ?? string.Empty;

      FontSpec = new FontSpec(fontFamily, fontSize, color, isBold, isItalic, isUnderline);
      IsVisible = true;
    }

    /// <summary>
    /// Constructor that builds a <see cref="Label" /> from a text <see cref="string" />
    /// and a <see cref="ZedGraph.FontSpec" /> instance.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fontSpec"></param>
    public Label(string text, FontSpec fontSpec)
    {
      _text = text ?? string.Empty;

      FontSpec = fontSpec;
      IsVisible = true;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="rhs">the <see cref="Label" /> instance to be copied.</param>
    public Label(Label rhs)
    {
      if (rhs.Text != null)
        _text = (string)rhs.Text.Clone();
      else
        _text = string.Empty;

      IsVisible = rhs.IsVisible;
      if (rhs.FontSpec != null)
        FontSpec = rhs.FontSpec.Clone();
      else
        FontSpec = null;
    }

    /// <summary>
    /// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    /// calling the typed version of <see cref="Clone" />
    /// </summary>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone()
    {
      return this.Clone();
    }

    /// <summary>
    /// Typesafe, deep-copy clone method.
    /// </summary>
    /// <returns>A new, independent copy of this class</returns>
    public Label Clone()
    {
      return new Label(this);
    }

    #endregion

    #region Fields

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    private const int _schema = 10;

    /// <summary>
    /// Storage field for Text property
    /// </summary>
    internal string _text;

    #endregion

    #region Properties

    /// <summary>
    /// The <see cref="String" /> text to be displayed
    /// </summary>
    public string Text
    {
      get { return _text; }
      set { _text = value ?? string.Empty; }
    }

    /// <summary>
    /// A <see cref="ZedGraph.FontSpec" /> instance representing the font properties
    /// for the displayed text.
    /// </summary>
    public FontSpec FontSpec { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that determines whether or not this label will be displayed.
    /// </summary>
    public bool IsVisible { get; set; }

    #endregion

    #region Serialization

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected Label(SerializationInfo info, StreamingContext context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32("schema");

      _text = info.GetString("text");
      IsVisible = info.GetBoolean("isVisible");
      FontSpec = (FontSpec)info.GetValue("fontSpec", typeof(FontSpec));
    }
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("schema", _schema);
      info.AddValue("text", Text);
      info.AddValue("isVisible", IsVisible);
      info.AddValue("fontSpec", FontSpec);
    }
    #endregion


  }
}
