Feature: Only success stories for contact actions

@contactApp
Scenario: Edit a contact
	Given exist valid contact model
		And edit the contact
	When call edit request
		And search for contact
	Then the right contact results returns

Scenario: Add and Delete a contact
	Given random valid contact model
	When call create request
		And search for contact
	Then the right contact results returns
	When call delete request
		And search for contact
	Then contact removed
