import { HttpResponse, type HttpResponseResolver } from 'msw';

import type { PresentWidgetData } from '$main/account/save-editor/presentTypes.ts';

export const handlePresentData: HttpResponseResolver = () => {
  return HttpResponse.json<PresentWidgetData>({
    types: [
      {
        value: 'Material',
        label: 'Material',
        hasQuantity: true
      },
      {
        value: 'Chara',
        label: 'Adventurer',
        hasQuantity: false
      },
      {
        value: 'DmodePoint',
        label: 'Kaleidoscape Points',
        hasQuantity: true
      }
    ],
    //{ Material: 'Material', Chara: 'Adventurer', DmodePoint: 'Kaleidoscape points' },
    availableItems: {
      Material: [
        {
          value: 101001003,
          label: 'Gold Crystal'
        }
      ],
      Chara: [
        {
          value: 10150202,
          label: 'Summer Celliera'
        }
      ],
      DmodePoint: [
        {
          value: 10001,
          label: 'Dawn Amber'
        },
        {
          value: 10002,
          label: 'Dusk Amber'
        }
      ]
    }
  });
};
