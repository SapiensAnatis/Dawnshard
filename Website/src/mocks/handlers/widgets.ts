import { HttpResponse, type HttpResponseResolver } from 'msw';

import type { PresentWidgetData } from '$main/account/save-editor/present/presentTypes.ts';

export const handlePresentData: HttpResponseResolver = () => {
  return HttpResponse.json<PresentWidgetData>({
    types: [
      {
        type: 'Material',
        maxQuantity: 999_999
      },
      {
        type: 'Chara',
        maxQuantity: 1
      },
      {
        type: 'DmodePoint',
        maxQuantity: 999_999
      }
    ],
    availableItems: {
      Material: [
        {
          id: 101001003
        }
      ],
      Chara: [
        {
          id: 10150202
        }
      ],
      DmodePoint: [
        {
          id: 10001
        },
        {
          id: 10002
        }
      ],
      Dragon: [],
      Wyrmite: [],
      Wyrmprint: [],
      Rupies: [],
      Item: [],
      SkipTicket: [],
      DragonGift: [],
      FreeDiamantium: [],
      HustleHammer: [],
      Dew: [],
      WeaponBody: [],
      WeaponSkin: []
    }
  });
};
