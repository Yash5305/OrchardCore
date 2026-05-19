using Microsoft.Extensions.Localization;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentFields.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Mvc.ModelBinding;

namespace OrchardCore.ContentFields.Drivers;

public sealed class TextFieldDisplayDriver : ContentFieldDisplayDriver<TextField>
{
    private const string TelEditor = "Tel";

    private readonly IPhoneFormatValidator _phoneFormatValidator;

    internal readonly IStringLocalizer S;

    public TextFieldDisplayDriver(
        IPhoneFormatValidator phoneFormatValidator,
        IStringLocalizer<TextFieldDisplayDriver> localizer)
    {
        _phoneFormatValidator = phoneFormatValidator;
        S = localizer;
    }

    public override IDisplayResult Display(TextField field, BuildFieldDisplayContext context)
    {
        return Initialize<DisplayTextFieldViewModel>(GetDisplayShapeType(context), model =>
        {
            model.Field = field;
            model.Part = context.ContentPart;
            model.PartFieldDefinition = context.PartFieldDefinition;
        })
        .Location(OrchardCoreConstants.DisplayType.Detail, "Content")
        .Location(OrchardCoreConstants.DisplayType.Summary, "Content");
    }

    public override IDisplayResult Edit(TextField field, BuildFieldEditorContext context)
    {
        return Initialize<EditTextFieldViewModel>(GetEditorShapeType(context), model =>
        {
            var settings = context.PartFieldDefinition.GetSettings<TextFieldSettings>();
            model.Text = context.IsNew && field.Text == null ? settings.DefaultValue : field.Text;
            model.Field = field;
            model.Part = context.ContentPart;
            model.PartFieldDefinition = context.PartFieldDefinition;
        });
    }

    public override async Task<IDisplayResult> UpdateAsync(TextField field, UpdateFieldEditorContext context)
    {
        await context.Updater.TryUpdateModelAsync(field, Prefix, f => f.Text);
        var settings = context.PartFieldDefinition.GetSettings<TextFieldSettings>();

        if (settings.Required && string.IsNullOrWhiteSpace(field.Text))
        {
            context.Updater.ModelState.AddModelError(Prefix, nameof(field.Text), S["A value is required for {0}.", context.PartFieldDefinition.DisplayName()]);
        }

        // if editor is tel && field is not empty && phone validator says it is invalid
        if(GetEditorShapeType(context).EndsWith(TelEditor) && !string.IsNullOrWhiteSpace(field.Text) && !_phoneFormatValidator.IsValid(field.Text))
        {
            context.Updater.ModelState.AddModelError(Prefix, nameof(field.Text), S["The phone number is invalid for {0}.", context.PartFieldDefinition.DisplayName()]);

        }
        // show form error

        return Edit(field, context);
    }
}
