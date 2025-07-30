Feature: Credit Card Validation
  As a user of the payment API
  I want to validate credit card details
  So that I can ensure only valid payment information is processed

@success
Scenario Outline: Successful validation of different card types
  Given I have a credit card with the following details:
    | Field  | Value              |
    | Owner  | <Owner>            |
    | Number | <Number>           |
    | Date   | <Date>             |
    | Cvv    | <Cvv>              |
  When I request to validate the credit card
  Then the response status code should be 200
  And the response body should be "<CardType>"
  Examples:
    | Owner      | Number             | Date  | Cvv  | CardType        |
    | John Doe   | 4111111111111111   | 12/28 | 123  | Visa            |
    | Jane Smith | 5511111111111111   | 11/29 | 456  | MasterCard      |
    | J Doe      | 371111111111111    | 10/30 | 7890 | AmericanExpress |

@failure
Scenario Outline: Validation fails due to a single invalid field
  Given I have a valid credit card
  And the "<Field>" is "<InvalidValue>"
  When I request to validate the credit card
  Then the response status code should be 400
  And the response should contain the error "<ErrorMessage>" for the "<Field>" field
  Examples:
    | Field  | InvalidValue        | ErrorMessage  |
    | Owner  | John Doe 123        | Wrong owner   |
    | Number | 1234567890          | Wrong number  |
    | Date   | 01/20               | Wrong date    |
    | Cvv    | 12                  | Wrong cvv     |

@failure
Scenario: Validation fails when a required field is missing
  Given I have a valid credit card
  And the "Owner" field is missing
  When I request to validate the credit card
  Then the response status code should be 400
  And the response should contain the error "Owner is required" for the "Owner" field

  @failure
Scenario: Validation fails and returns all errors
  Given I have a credit card with an invalid "Date" of "13/25" and an invalid "Cvv" of "12"
  When I request to validate the credit card
  Then the response status code should be 400
  And the response should contain the error "Wrong date" for the "Date" field
  And the response should contain the error "Wrong cvv" for the "Cvv" field