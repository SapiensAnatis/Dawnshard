import { HttpResponse, type HttpResponseResolver } from 'msw';

import type { PresentWidgetData } from '$main/account/save-editor/present/presentTypes.ts';

export const handlePresentData: HttpResponseResolver = () => {
  return HttpResponse.json<PresentWidgetData>({
    types: [
      {
        type: 'Material',
        hasQuantity: true
      },
      {
        type: 'Chara',
        hasQuantity: false
      },
      {
        type: 'DmodePoint',
        hasQuantity: true
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
      ]
    }
  });
};
