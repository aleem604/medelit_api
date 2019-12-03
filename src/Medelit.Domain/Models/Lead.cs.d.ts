declare module server {
	interface lead extends baseEntity {
		surName: string;
		titleId?: number;
		name: string;
		invoiceEntityId?: number;
		mainPhone: string;
		mainPhoneOwner: string;
		phone2: string;
		phone2Owner: string;
		phone3: string;
		phone3Owner: string;
		contactPhone: string;
		visitRequestingPerson: string;
		visitRequestingPersonRelationId?: number;
		fax: string;
		email: string;
		leadSourceId?: number;
		leadStatusId?: number;
		languageId?: number;
		leadCategoryId?: number;
		contactMethod?: number;
		dateOfBirth?: Date;
		countryOfBirthId?: number;
		preferredPaymentMethodId?: number;
		invoicingNotes: string;
		insuranceCover?: boolean;
		listedDiscountNetworkId?: number;
		discount?: number;
		gPCode: string;
		addressStreetName: string;
		postalCode: string;
		cityId?: number;
		countryId?: number;
		buildingTypeId?: number;
		flatNumber?: number;
		buzzer: string;
		floor?: number;
		visitVenueId?: number;
		addressNotes: string;
		visitVenueDetail: string;
		fromCustomerId?: number;
		convertDate?: Date;
	}
}
