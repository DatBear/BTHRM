import { IconDefinition } from "@fortawesome/fontawesome-svg-core";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import clsx from "clsx";

type SectionProps = {
  value: number | string;
  name: string;
  icon: IconDefinition;
  iconColor: string;
}

export default function Section({ value, name, icon, iconColor }: SectionProps) {
  return <div className="flex flex-row items-center gap-4">
    <FontAwesomeIcon icon={icon} className={clsx(iconColor, 'text-3xl')} />
    <div className="flex flex-col">
      <div className="text-4xl">{value}</div>
      <div className="text-md">{name}</div>
    </div>
  </div>
}