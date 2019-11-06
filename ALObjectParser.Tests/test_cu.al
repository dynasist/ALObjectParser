page 88971 "ATDDTestFeatures_DSK"
{
    Caption = 'ATDD Test Features';
    PageType = List;
    SourceTable = "ATDDTestFeature_DSK";
    UsageCategory = Lists;
    ApplicationArea = All;

    layout
    {
        area(content)
        {
            repeater(Group)
            {

                field("Code"; "Code")
                {
                    ApplicationArea = All;
                    //Caption = 'Code';
                }

                field("Name"; "Name")
                {
                    ApplicationArea = All;
                    //Caption = 'Name';
                }

            }
        }
    }

	actions {
		action(x) {
			ApplicationArea = All;
		}
	}
}

codeunit 81000 "LookupValue UT Customer"
{
    // Generated on 2019. 09. 28. at 22:11 by MártonSági
    
    // [FEATURE] LookupValue UT Customer
    SubType = Test;
    
    [Test]
    procedure CheckThatLabelCanBeAssignedToCustomer(var Rec: Record "Sales Header"): Boolean
    // [FEATURE] LookupValue UT Customer
    begin
        // [SCENARIO #0001] Check that label can be assigned to customer
        Initialize();
        
        // [GIVEN] A label
        CreateALabel();
        
        // [GIVEN] A customer
        CreateACustomer();
        
        // [WHEN] Assign label to customer
        AssignLabelToCustomer();
        
        // [THEN] Customer has label field populated
        VerifyCustomerHasLabelFieldPopulated();
        
    end;
    
    [Test]
    procedure CheckThatLabelFieldTableRelationIsValidatedForNonExistingLabelOnCustomer()
    // [FEATURE] LookupValue UT Customer
    begin
        // [SCENARIO #0002] Check that label field table relation is validated for non-existing label on customer
        Initialize();
        
        // [GIVEN] A non-existing label value
        CreateANonExistingLabelValue();
        
        // [GIVEN] A customer record variable
        CreateACustomerRecordVariable();
        
        // [WHEN] Assign non-existing label to customer
        AssignNonExistingLabelToCustomer();
        
        // [THEN] Non existing label error was thrown
        VerifyNonExistingLabelErrorWasThrown();
        
    end;
    
    [Test]
    procedure CheckThatLabelCanBeAssignedOnCustomerCard()
    // [FEATURE] LookupValue UT Customer
    begin
        // [SCENARIO #0003] Check that label can be assigned on customer card
        Initialize();
        
        // [GIVEN] A label
        CreateALabel();
        
        // [GIVEN] A customer card
        CreateACustomerCard();
        
        // [WHEN] Assign label to customer card
        AssignLabelToCustomerCard();
        
        // [THEN] Customer has label field populated
        VerifyCustomerHasLabelFieldPopulated();
        
    end;
    
    var
        IsInitialized: Boolean;
    
    local procedure Initialize()
    var
        LibraryTestInitialize: Codeunit "Library - Test Initialize";
    begin
        LibraryTestInitialize.OnTestInitialize(Codeunit::"LookupValue UT Customer");
        
        if IsInitialized then
            exit;
        
        LibraryTestInitialize.OnBeforeTestSuiteInitialize(Codeunit::"LookupValue UT Customer");
        
        IsInitialized := true;
        Commit();
        
        LibraryTestInitialize.OnAfterTestSuiteInitialize(Codeunit::"LookupValue UT Customer");
    end;
    
    local procedure AssignLabelToCustomer()
    begin
    end;
    
    local procedure AssignLabelToCustomerCard()
    begin
    end;
    
    local procedure AssignNonExistingLabelToCustomer()
    begin
    end;
    
    local procedure CreateACustomer()
    begin
    end;
    
    local procedure CreateACustomerCard()
    begin
    end;
    
    local procedure CreateACustomerRecordVariable()
    begin
    end;
    
    local procedure CreateALabel()
    begin
    end;
    
    local procedure CreateANonExistingLabelValue()
    begin
    end;
    
    local procedure VerifyCustomerHasLabelFieldPopulated()
    begin
    end;
    
    local procedure VerifyNonExistingLabelErrorWasThrown()
    begin
    end;
    
}
