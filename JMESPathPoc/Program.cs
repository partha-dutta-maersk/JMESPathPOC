
using JsonQueryPOC;
using JsonQueryPOC.Services;
using DevLab.JmesPath;

var query = GetQuery();

const string url = @"history/Booking_67809_2b0a5605-a709-49e2-a694-372691d2a5ec_bookingupdate";

var blobProvider = new BlobDataProvider();
var jsonData = await blobProvider.Get<string>(url, BlobSerializationType.None);

var jmes = new JmesPath();
var result = jmes.Transform(jsonData, query);

Console.ReadLine();

string GetQuery()
{
    const string expression = @"{
CreateUserId: CreateUserId,
CreateUserEmail: CreateUserEmail,
ShipperBookingNumber: ShipperBookingNumber,
DataObject: DataObject,
DataObjectAcronym: DataObjectAcronym,
ExternalBookingIdentifier: ExternalBookingIdentifier,
UpdateUserId: UpdateUserId,
UpdateTimestamp: UpdateTimestamp,
OwningOfficePartyCode: OwningOfficePartyCode,
ShipperBookingStatus: ShipperBookingStatus,
TotalBookedPackageQuantity: TotalBookedPackageQuantity,
TotalBookedCustomerProductQuantity: TotalBookedCustomerProductQuantity,
TotalBookedWeight: TotalBookedWeight,
TotalBookedVolume: TotalBookedVolume,
Parties: Parties[?PartyFunction == 'CONSIGNEE' ||
                  PartyFunction == 'SHIPPER' ||
                  PartyFunction == 'SHIPPER'].{
    PartyFunction: PartyFunction,
    Party: Party.{
        PartyCode: PartyCode,
        BusinessEntityCountryCode: BusinessEntityCountryCode,
        BusinessEntityCode: BusinessEntityCode,
        BusinessEntityFunction: BusinessEntityFunction
    }
},
Locations: Locations[].{
    LocationFunction: LocationFunction,
    Location: Location.{
        FacilityCode: FacilityCode,
        FacilityName: FacilityName
    }
},
ServicePlan: ServicePlan.{
    Incoterm: Incoterm,
    TransportMode: TransportMode,
    FreightTerm: FreightTerm,
    ServiceTypeModes: ServiceTypeModes[].{
        LoadService: LoadService,
        DischargeService: DischargeService
    }
},
CreateSource: CreateSource,
ContainerGohFlag: length(ShipperBookingLines[?CustomerOrderLine.CargoType == 'GARMENT_ON_HANGERS']),
ContainerDgFlag: length(ShipperBookingLines[?CustomerOrderLine.CargoType == 'DANGEROUS'])
}";
    return expression;
}



